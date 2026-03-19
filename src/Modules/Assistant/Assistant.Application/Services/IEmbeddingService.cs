namespace Assistant.Application.Services;

public interface IEmbeddingService
{
    Task<float[]> CreateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
}
