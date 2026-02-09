using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Data.ConfigurationData
{
    public class DepartmentMemberConfiguration : IEntityTypeConfiguration<DepartmentMember>
    {
        public void Configure(EntityTypeBuilder<DepartmentMember> builder)
        {
            builder.HasKey(dm => dm.DepartmentMemberId);

            builder.HasOne(dm => dm.Department)
                   .WithMany(d => d.DepartmentMembers)
                   .HasForeignKey(dm => dm.DepartmentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dm => dm.User)
                   .WithMany(u => u.DepartmentMembers)
                   .HasForeignKey(dm => dm.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dm => dm.DepartmentRole)
                   .WithMany()
                   .HasForeignKey(dm => dm.DepartmentRoleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
