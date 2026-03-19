namespace Assistant.Application.Configuration;

public sealed class OpenRouterOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string ModelId { get; set; } = "google/gemma-3-27b-it:free";
    public string[] FallbackModels { get; set; } = [];
    public string SiteUrl { get; set; } = string.Empty;
    public string AppName { get; set; } = "TaskFlow Assistant";
    public double Temperature { get; set; } = 0.2d;
    public int MaxCompletionTokens { get; set; } = 600;
}
