using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data.SeedData
{
    public class DepartmentRoleDataConfiguration : IEntityTypeConfiguration<DepartmentRole>
    {
        public void Configure(EntityTypeBuilder<DepartmentRole> builder)
        {
            builder.HasData(
                new DepartmentRole { Id = 1, Name = "Lider" },
                new DepartmentRole { Id = 2, Name = "Yardımcı Lider" },
                new DepartmentRole { Id = 3, Name = "Çalışan" }
            );
        }
    }
}
