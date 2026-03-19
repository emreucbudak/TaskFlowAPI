using Assistant.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace Assistant.Infrastructure.Services;

public sealed class GeminiAssistantCompletionClient(
    Kernel kernel,
    IOptions<GeminiOptions> geminiOptions,
    ILogger<GeminiAssistantCompletionClient> logger)
{
    public async Task<string?> CompleteAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        var apiKey = geminiOptions.Value.ApiKey?.Trim();
        if (string.IsNullOrWhiteSpace(apiKey) || string.Equals(apiKey, "__FROM_SECRET__", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Gemini ApiKey tanimli degil. Assistant Gemini fallback calistirilamiyor.");
        }

        var prompt = $$"""
            {{systemPrompt}}

            <KULLANICI_ISTEGI>
            {{userPrompt}}
            </KULLANICI_ISTEGI>
            """;

        try
        {
            var result = await kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);
            return result.GetValue<string>()?.Trim();
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Assistant Gemini fallback cevabi olusturulamadi.");
            throw;
        }
    }
}
