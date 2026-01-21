using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data.SeedData
{
    public class GroupRolesDataConfiguration : IEntityTypeConfiguration<GroupRoles>
    {
        public void Configure(EntityTypeBuilder<GroupRoles> builder)
        {
            builder.HasData(
                new GroupRoles { GroupRolesId = 1, RoleName = "Leader" },
                new GroupRoles { GroupRolesId = 2, RoleName = "User" },
                new GroupRoles { GroupRolesId = 3, RoleName = "Manager" }
            );
        }
    }
}
