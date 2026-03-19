using Assistant.Domain.Entities;
using Assistant.Persistence.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Assistant.Persistence.Data.AssistantDb;

public sealed class AssistantDbContext : DbContext
{
    public AssistantDbContext(DbContextOptions<AssistantDbContext> options) : base(options)
    {
    }

    public DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();
    public DbSet<KnowledgeChunk> KnowledgeChunks => Set<KnowledgeChunk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfiguration(new KnowledgeDocumentConfiguration());
        modelBuilder.ApplyConfiguration(new KnowledgeChunkConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
