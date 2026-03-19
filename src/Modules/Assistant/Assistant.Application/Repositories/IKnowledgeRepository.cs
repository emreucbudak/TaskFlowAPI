using Assistant.Application.Models;
using Assistant.Domain.Entities;

namespace Assistant.Application.Repositories;

public interface IKnowledgeRepository
{
    Task<KnowledgeDocument?> GetDocumentBySourceKeyAsync(string sourceKey, CancellationToken cancellationToken = default);
    Task UpsertDocumentAsync(KnowledgeDocument document, CancellationToken cancellationToken = default);
    Task ReplaceChunksAsync(Guid documentId, IReadOnlyCollection<KnowledgeChunk> chunks, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<KnowledgeSearchResult>> SearchSimilarChunksAsync(float[] embedding, int topK, CancellationToken cancellationToken = default);
}
