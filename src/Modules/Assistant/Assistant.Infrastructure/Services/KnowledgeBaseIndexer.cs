using Assistant.Application.Configuration;
using Assistant.Application.Models;
using Assistant.Application.Repositories;
using Assistant.Application.Services;
using Assistant.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Assistant.Infrastructure.Services;

public sealed class KnowledgeBaseIndexer(
    IKnowledgeBaseSourceReader sourceReader,
    ITextChunker textChunker,
    IEmbeddingService embeddingService,
    IKnowledgeRepository knowledgeRepository,
    IOptions<AssistantOptions> assistantOptions) : IKnowledgeBaseIndexer
{
    public async Task<KnowledgeIndexResult> IndexAsync(CancellationToken cancellationToken = default)
    {
        var documents = await sourceReader.ReadAllAsync(cancellationToken);
        var filesProcessed = 0;
        var filesSkipped = 0;
        var chunksIndexed = 0;

        foreach (var document in documents)
        {
            filesProcessed++;

            var existingDocument = await knowledgeRepository.GetDocumentBySourceKeyAsync(document.SourceKey, cancellationToken);
            if (existingDocument is not null && string.Equals(existingDocument.Checksum, document.Checksum, StringComparison.Ordinal))
            {
                filesSkipped++;
                continue;
            }

            var now = DateTimeOffset.UtcNow;
            var documentId = existingDocument?.Id ?? Guid.NewGuid();
            var knowledgeDocument = new KnowledgeDocument(
                documentId,
                document.SourceKey,
                document.Title,
                document.Checksum,
                existingDocument?.CreatedAt ?? now,
                now);

            await knowledgeRepository.UpsertDocumentAsync(knowledgeDocument, cancellationToken);

            var chunkTexts = textChunker.Split(
                document.Content,
                assistantOptions.Value.ChunkSize,
                assistantOptions.Value.ChunkOverlap);

            var chunks = new List<KnowledgeChunk>(chunkTexts.Count);
            for (var index = 0; index < chunkTexts.Count; index++)
            {
                var embedding = await embeddingService.CreateDocumentEmbeddingAsync(
                    chunkTexts[index],
                    document.Title,
                    cancellationToken);
                ValidateEmbeddingDimensions(embedding, assistantOptions.Value.EmbeddingDimensions);

                chunks.Add(new KnowledgeChunk(
                    Guid.NewGuid(),
                    documentId,
                    index,
                    chunkTexts[index],
                    embedding,
                    null,
                    now));
            }

            await knowledgeRepository.ReplaceChunksAsync(documentId, chunks, cancellationToken);
            chunksIndexed += chunks.Count;
        }

        return new KnowledgeIndexResult(filesProcessed, filesSkipped, chunksIndexed);
    }

    private static void ValidateEmbeddingDimensions(float[] embedding, int expectedDimensions)
    {
        if (embedding.Length != expectedDimensions)
        {
            throw new InvalidOperationException(
                $"Embedding boyutu uyusmuyor. Beklenen: {expectedDimensions}, gelen: {embedding.Length}.");
        }
    }
}
