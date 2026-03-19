using Assistant.Application.Configuration;
using Assistant.Application.Models;
using Assistant.Application.Repositories;
using Assistant.Application.Services;
using Microsoft.Extensions.Options;

namespace Assistant.Infrastructure.Services;

public sealed class KnowledgeSearchService(
    IEmbeddingService embeddingService,
    IKnowledgeRepository knowledgeRepository,
    IOptions<AssistantOptions> assistantOptions) : IKnowledgeSearchService
{
    public async Task<IReadOnlyCollection<KnowledgeSearchResult>> SearchAsync(string query, int? topK = null, CancellationToken cancellationToken = default)
    {
        var queryEmbedding = await embeddingService.CreateQueryEmbeddingAsync(query, cancellationToken);
        ValidateEmbeddingDimensions(queryEmbedding, assistantOptions.Value.EmbeddingDimensions);
        var take = topK.GetValueOrDefault(assistantOptions.Value.SearchTopK);
        return await knowledgeRepository.SearchSimilarChunksAsync(queryEmbedding, take, cancellationToken);
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
