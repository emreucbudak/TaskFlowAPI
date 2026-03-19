using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Data.ConfigurationData
{
    public class GroupsMemberConfiguration : IEntityTypeConfiguration<GroupsMember>
    {
        public void Configure(EntityTypeBuilder<GroupsMember> builder)
        {
            builder.HasKey(gm => gm.Id);

            builder.Property(gm => gm.Id)
                   .HasColumnName("GroupsMemberId");

            builder.HasOne(gm => gm.Group)
                   .WithMany()
                   .HasForeignKey(gm => gm.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(gm => gm.User)
                   .WithMany()
                   .HasForeignKey(gm => gm.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(gm => gm.GroupRoles)
                   .WithMany()
                   .HasForeignKey(gm => gm.GroupRolesId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
