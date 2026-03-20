using Assistant.Application.Models;
using Assistant.Application.Services;
using FlashMediator;

namespace Assistant.Application.Features.CQRS.Chat.Queries.Ask;

public sealed class AskChatbotQueryHandler(
    IAssistantChatService assistantChatService)
    : IRequestHandler<AssistantChatRequest, AssistantChatResponseDto>
{
    public async Task<AssistantChatResponseDto> Handle(
        AssistantChatRequest request, CancellationToken cancellationToken)
    {
        return await assistantChatService.AskAsync(request.Question, cancellationToken);
    }
}
