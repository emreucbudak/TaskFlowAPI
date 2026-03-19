namespace Assistant.Application.Models;

public sealed record KnowledgeBaseFile(
    string SourceKey,
    string Title,
    string Content,
    string Checksum);
