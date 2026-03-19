namespace Assistant.Application.Models;

public sealed record KnowledgeSearchResult(
    Guid DocumentId,
    string SourceKey,
    string Title,
    Guid ChunkId,
    int ChunkIndex,
    string ChunkText,
    double Score);
