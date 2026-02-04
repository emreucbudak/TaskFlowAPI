using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Models;

namespace Notification.Persistence.Data.DataConfiguration
{
    public class NotificationMessageConfiguration : IEntityTypeConfiguration<NotificationMessage>
    {
        public void Configure(EntityTypeBuilder<NotificationMessage> builder)
        {
            builder.HasKey(x => x.Id);


            builder.HasIndex(x => x.ReceiverUserId);
            builder.HasIndex(x => x.SendTime);
            builder.HasIndex(x => x.IsRead);


            builder.HasIndex(x => new { x.ReceiverUserId, x.SendTime })
                .IsDescending(false, true);
        }
    }
}
