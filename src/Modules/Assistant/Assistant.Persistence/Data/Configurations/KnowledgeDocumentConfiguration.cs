using Assistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assistant.Persistence.Data.Configurations;

public sealed class KnowledgeDocumentConfiguration : IEntityTypeConfiguration<KnowledgeDocument>
{
    public void Configure(EntityTypeBuilder<KnowledgeDocument> builder)
    {
        builder.ToTable("rag_documents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.SourceKey)
            .HasColumnName("source_key")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.HasIndex(x => x.SourceKey)
            .HasDatabaseName("uq_rag_documents_source_key")
            .IsUnique();

        builder.HasMany(x => x.Chunks)
            .WithOne(x => x.Document)
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(KnowledgeDocument.Chunks))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
