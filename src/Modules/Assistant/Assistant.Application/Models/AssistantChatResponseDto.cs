namespace Assistant.Application.Models;

public sealed record AssistantChatResponseDto(
    string Answer,
    IReadOnlyCollection<AssistantChatSourceDto> Sources);

public sealed record AssistantChatSourceDto(
    string SourceKey,
    string Title,
    int ChunkIndex,
    double Score);
