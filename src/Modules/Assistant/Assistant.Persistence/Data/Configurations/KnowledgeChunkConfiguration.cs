using Assistant.Application.Configuration;
using Assistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assistant.Persistence.Data.Configurations;

public sealed class KnowledgeChunkConfiguration : IEntityTypeConfiguration<KnowledgeChunk>
{
    public void Configure(EntityTypeBuilder<KnowledgeChunk> builder)
    {
        builder.ToTable("rag_chunks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.DocumentId)
            .HasColumnName("document_id");

        builder.Property(x => x.ChunkIndex)
            .HasColumnName("chunk_index")
            .IsRequired();

        builder.Property(x => x.ChunkText)
            .HasColumnName("chunk_text")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Embedding)
            .HasColumnName("embedding")
            .HasColumnType($"vector({AssistantOptions.DefaultEmbeddingDimensions})")
            .IsRequired();

        builder.Property(x => x.MetadataJson)
            .HasColumnName("metadata")
            .HasColumnType("jsonb");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.HasIndex(x => x.DocumentId)
            .HasDatabaseName("ix_rag_chunks_document_id");

        builder.HasIndex(x => new { x.DocumentId, x.ChunkIndex })
            .HasDatabaseName("uq_rag_chunks_document_chunk")
            .IsUnique();

        builder.HasIndex(x => x.Embedding)
            .HasDatabaseName("ix_rag_chunks_embedding")
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }
}
