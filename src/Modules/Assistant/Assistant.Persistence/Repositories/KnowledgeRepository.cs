using Assistant.Application.Models;
using Assistant.Application.Repositories;
using Assistant.Domain.Entities;
using Assistant.Persistence.Data.AssistantDb;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

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
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        await context.KnowledgeChunks
            .Where(chunk => chunk.DocumentId == documentId)
            .ExecuteDeleteAsync(cancellationToken);

        await context.KnowledgeChunks.AddRangeAsync(chunks, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<KnowledgeSearchResult>> SearchSimilarChunksAsync(float[] embedding, int topK, CancellationToken cancellationToken = default)
    {
        var queryVector = new Vector(embedding);

        var rawResults = await context.KnowledgeChunks
            .AsNoTracking()
            .Select(chunk => new
            {
                chunk.DocumentId,
                chunk.Id,
                chunk.ChunkIndex,
                chunk.ChunkText,
                chunk.Document.SourceKey,
                chunk.Document.Title,
                Distance = chunk.Embedding.CosineDistance(queryVector)
            })
            .OrderBy(item => item.Distance)
            .Take(topK)
            .ToListAsync(cancellationToken);

        return rawResults
            .Select(item => new KnowledgeSearchResult(
                item.DocumentId,
                item.SourceKey,
                item.Title,
                item.Id,
                item.ChunkIndex,
                item.ChunkText,
                1d - item.Distance))
            .ToList();
    }
}
