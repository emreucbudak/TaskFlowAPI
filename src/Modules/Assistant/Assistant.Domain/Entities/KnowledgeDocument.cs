namespace Assistant.Domain.Entities;

public sealed class KnowledgeDocument
{
    private readonly List<KnowledgeChunk> _chunks = [];

    private KnowledgeDocument()
    {
    }

    public KnowledgeDocument(
        Guid id,
        string sourceKey,
        string title,
        string checksum,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        SourceKey = sourceKey;
        Title = title;
        Checksum = checksum;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; private set; }
    public string SourceKey { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Checksum { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<KnowledgeChunk> Chunks => _chunks;

    public void Update(string title, string checksum, DateTimeOffset updatedAt)
    {
        Title = title;
        Checksum = checksum;
        UpdatedAt = updatedAt;
    }
}
