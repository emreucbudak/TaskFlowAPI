using Pgvector;

namespace Assistant.Domain.Entities;

public sealed class KnowledgeChunk
{
    private KnowledgeChunk()
    {
    }

    public KnowledgeChunk(
        Guid id,
        Guid documentId,
        int chunkIndex,
        string chunkText,
        float[] embedding,
        string? metadataJson,
        DateTimeOffset? createdAt = null)
    {
        Id = id;
        DocumentId = documentId;
        ChunkIndex = chunkIndex;
        ChunkText = chunkText;
        Embedding = new Vector(embedding);
        MetadataJson = metadataJson;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public KnowledgeDocument Document { get; private set; } = null!;
    public int ChunkIndex { get; private set; }
    public string ChunkText { get; private set; } = string.Empty;
    public Vector Embedding { get; private set; } = null!;
    public string? MetadataJson { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}
