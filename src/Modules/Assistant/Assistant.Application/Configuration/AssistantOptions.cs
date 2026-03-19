namespace Assistant.Application.Configuration;

public sealed class AssistantOptions
{
    public const int DefaultEmbeddingDimensions = 1536;

    public string KnowledgeBasePath { get; set; } = "KnowledgeBase";
    public int ChunkSize { get; set; } = 900;
    public int ChunkOverlap { get; set; } = 150;
    public int SearchTopK { get; set; } = 5;
    public double MinimumSourceScore { get; set; } = 0.55d;
    public int EmbeddingDimensions { get; set; } = DefaultEmbeddingDimensions;
}
