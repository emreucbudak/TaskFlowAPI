using Assistant.Application.Models;

namespace Assistant.Application.Services;

public interface IAssistantChatService
{
    Task<AssistantChatResponse> AskAsync(string question, CancellationToken cancellationToken = default);
}
