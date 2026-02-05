using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Chat.Domain.Entities;

namespace Chat.Persistence.Data.DataConfiguration
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Content).IsRequired();

        
            builder.HasIndex(e => new { e.GroupId, e.SendTime });

  
            builder.HasIndex(e => new { e.ReceiverId, e.SenderId, e.SendTime });

            builder.HasIndex(e => new { e.ReceiverId, e.IsRead });
        }
    }
}
