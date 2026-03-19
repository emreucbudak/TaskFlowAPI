using Assistant.Application.Services;
using Microsoft.Extensions.Logging;

namespace Assistant.Infrastructure.Services;

public sealed class AssistantCompletionClient(
    OpenRouterAssistantCompletionClient openRouterAssistantCompletionClient,
    GeminiAssistantCompletionClient geminiAssistantCompletionClient,
    ILogger<AssistantCompletionClient> logger) : IAssistantCompletionClient
{
    public async Task<string?> CompleteAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var openRouterAnswer = await openRouterAssistantCompletionClient.CompleteAsync(
                systemPrompt,
                userPrompt,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(openRouterAnswer))
            {
                return openRouterAnswer;
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Assistant OpenRouter generation basarisiz oldu. Gemini fallback denenecek.");
        }

        return await geminiAssistantCompletionClient.CompleteAsync(systemPrompt, userPrompt, cancellationToken);
    }
}
