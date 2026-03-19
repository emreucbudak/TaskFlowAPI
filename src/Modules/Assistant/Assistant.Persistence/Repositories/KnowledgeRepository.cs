using Assistant.Application.Models;
using Assistant.Application.Repositories;
using Assistant.Domain.Entities;
using Assistant.Persistence.Data.AssistantDb;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pgvector;
using System.Globalization;
using System.Data;

namespace Assistant.Persistence.Repositories;

public sealed class KnowledgeRepository(AssistantDbContext context) : IKnowledgeRepository
{
    public Task<KnowledgeDocument?> GetDocumentBySourceKeyAsync(string sourceKey, CancellationToken cancellationToken = default) =>
        context.KnowledgeDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(document => document.SourceKey == sourceKey, cancellationToken);

    public async Task UpsertDocumentAsync(KnowledgeDocument document, CancellationToken cancellationToken = default)
    {
        var existingDocument = await context.KnowledgeDocuments
            .FirstOrDefaultAsync(item => item.SourceKey == document.SourceKey, cancellationToken);

        if (existingDocument is null)
        {
            await context.KnowledgeDocuments.AddAsync(document, cancellationToken);
        }
        else
        {
            existingDocument.Update(document.Title, document.Checksum, document.UpdatedAt);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceChunksAsync(Guid documentId, IReadOnlyCollection<KnowledgeChunk> chunks, CancellationToken cancellationToken = default)
    {
        var connection = await GetOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        await using (var deleteCommand = new NpgsqlCommand(
                         "DELETE FROM rag_chunks WHERE document_id = @documentId;",
                         connection,
                         transaction))
        {
            deleteCommand.Parameters.AddWithValue("documentId", documentId);
            await deleteCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        const string insertSql = """
            INSERT INTO rag_chunks (id, chunk_index, chunk_text, created_at, document_id, embedding, metadata)
            VALUES (@id, @chunkIndex, @chunkText, @createdAt, @documentId, CAST(@embedding AS vector), CAST(@metadata AS jsonb));
            """;

        foreach (var chunk in chunks)
        {
            await using var insertCommand = new NpgsqlCommand(insertSql, connection, transaction);
            insertCommand.Parameters.AddWithValue("id", chunk.Id);
            insertCommand.Parameters.AddWithValue("chunkIndex", chunk.ChunkIndex);
            insertCommand.Parameters.AddWithValue("chunkText", chunk.ChunkText);
            insertCommand.Parameters.AddWithValue("createdAt", chunk.CreatedAt);
            insertCommand.Parameters.AddWithValue("documentId", chunk.DocumentId);
            insertCommand.Parameters.AddWithValue("embedding", ToVectorLiteral(chunk.Embedding));
            insertCommand.Parameters.AddWithValue("metadata", chunk.MetadataJson is null ? DBNull.Value : chunk.MetadataJson);
            await insertCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<KnowledgeSearchResult>> SearchSimilarChunksAsync(float[] embedding, int topK, CancellationToken cancellationToken = default)
    {
        var results = new List<KnowledgeSearchResult>();
        var queryVectorLiteral = ToVectorLiteral(new Vector(embedding));

        var connection = await GetOpenConnectionAsync(cancellationToken);

        const string searchSql = """
            SELECT
                c.document_id,
                d.source_key,
                d.title,
                c.id,
                c.chunk_index,
                c.chunk_text,
                1 - (c.embedding <=> CAST(@embedding AS vector)) AS score
            FROM rag_chunks AS c
            INNER JOIN rag_documents AS d ON d.id = c.document_id
            ORDER BY c.embedding <=> CAST(@embedding AS vector)
            LIMIT @topK;
            """;

        await using var searchCommand = new NpgsqlCommand(searchSql, connection);
        searchCommand.Parameters.AddWithValue("embedding", queryVectorLiteral);
        searchCommand.Parameters.AddWithValue("topK", topK);

        await using var reader = await searchCommand.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new KnowledgeSearchResult(
                reader.GetGuid(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetGuid(3),
                reader.GetInt32(4),
                reader.GetString(5),
                reader.GetDouble(6)));
        }

        return results;
    }

    private async Task<NpgsqlConnection> GetOpenConnectionAsync(CancellationToken cancellationToken)
    {
        var dbConnection = context.Database.GetDbConnection();
        if (dbConnection is not NpgsqlConnection npgsqlConnection)
        {
            throw new InvalidOperationException("Assistant vector baglantisi NpgsqlConnection degil.");
        }

        if (npgsqlConnection.State != ConnectionState.Open)
        {
            await context.Database.OpenConnectionAsync(cancellationToken);
        }

        return npgsqlConnection;
    }

    private static string ToVectorLiteral(Vector vector) =>
        $"[{string.Join(",", vector.ToArray().Select(value => value.ToString(CultureInfo.InvariantCulture)))}]";
}
