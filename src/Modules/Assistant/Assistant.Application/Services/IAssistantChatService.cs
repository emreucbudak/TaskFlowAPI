using Assistant.Application.Models;

namespace Assistant.Application.Services;

public interface IAssistantChatService
{
    Task<AssistantChatResponseDto> AskAsync(string question, CancellationToken cancellationToken = default);
}
