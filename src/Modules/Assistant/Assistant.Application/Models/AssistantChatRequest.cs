using FlashMediator;

namespace Assistant.Application.Models;

public sealed record AssistantChatRequest(string Question) : IRequest<AssistantChatResponseDto>;
