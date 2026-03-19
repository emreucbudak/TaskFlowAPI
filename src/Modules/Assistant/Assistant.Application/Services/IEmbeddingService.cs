namespace Assistant.Application.Services;

public interface IEmbeddingService
{
    Task<float[]> CreateDocumentEmbeddingAsync(string text, string? title = null, CancellationToken cancellationToken = default);
    Task<float[]> CreateQueryEmbeddingAsync(string text, CancellationToken cancellationToken = default);
}
