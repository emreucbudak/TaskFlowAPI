using Chat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Persistence.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(4000); // Reasonable limit for chat messages

            builder.Property(x => x.SenderId)
                .IsRequired();

            // Index for faster lookups
            builder.HasIndex(x => x.SenderId);
            builder.HasIndex(x => x.ReceiverId);
            builder.HasIndex(x => x.GroupId);
            builder.HasIndex(x => x.SendTime);

            // Configure properties mapping if needed, but standard convention should work
        }
    }
}