namespace Assistant.Application.Services;

public interface IAssistantCompletionClient
{
    Task<string?> CompleteAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default);
}
