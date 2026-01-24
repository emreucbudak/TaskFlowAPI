using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data.ConfigurationData
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(u => u.Department)
                   .WithMany(d=> d.Users)
                   .HasForeignKey(u => u.DepartmentId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
