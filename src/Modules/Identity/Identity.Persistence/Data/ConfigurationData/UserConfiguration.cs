using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Data.ConfigurationData
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u => u.DepartmentMembers)
                   .WithOne(dm => dm.User)
                   .HasForeignKey(dm => dm.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
