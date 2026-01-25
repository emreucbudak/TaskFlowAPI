using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data.SeedData
{
    public class RolesConfiguration : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            builder.HasData(
                new Roles
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-4789-9012-34567890abcd"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new Roles
                {
                    Id = Guid.Parse("b2c3d4e5-f678-4901-2345-67890abcdeff"),
                    Name = "Company",
                    NormalizedName = "COMPANY",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new Roles
                {
                    Id = Guid.Parse("c3d4e5f6-7890-1234-5678-90abcdef1234"),
                    Name = "Worker",
                    NormalizedName = "WORKER",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }

            );
        }
    }
}
