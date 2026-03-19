namespace Assistant.Application.Models;

public sealed record AssistantChatResponse(
    string Answer,
    IReadOnlyCollection<KnowledgeSearchResult> Sources);
