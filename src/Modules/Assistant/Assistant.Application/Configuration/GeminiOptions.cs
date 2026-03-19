namespace Assistant.Application.Configuration;

public sealed class GeminiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string ModelId { get; set; } = "gemini-3-flash-preview";
    public string EmbeddingModelId { get; set; } = "gemini-embedding-001";
}
