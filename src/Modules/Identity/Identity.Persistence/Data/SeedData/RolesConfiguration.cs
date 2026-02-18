using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Data.SeedData
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
                    ConcurrencyStamp = "c8f1c3b2-e4a5-4b6c-8d7e-9f0a1b2c3d4e"
                },
                new Roles
                {
                    Id = Guid.Parse("b2c3d4e5-f678-4901-2345-67890abcdeff"),
                    Name = "Company",
                    NormalizedName = "COMPANY",
                    ConcurrencyStamp = "d9g2d4c3-f5b6-5c7d-9e8f-0g1b2c3d4e5f"
                },
                new Roles
                {
                    Id = Guid.Parse("c3d4e5f6-7890-1234-5678-90abcdef1234"),
                    Name = "Worker",
                    NormalizedName = "WORKER",
                    ConcurrencyStamp = "e0h3e5d4-g6c7-6d8e-0f9g-1h2c3d4e5f6g"
                }

            );
        }
    }
}
