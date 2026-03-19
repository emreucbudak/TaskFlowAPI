namespace Assistant.Application.Models;

public sealed record KnowledgeIndexResult(
    int FilesProcessed,
    int FilesSkipped,
    int ChunksIndexed);
