using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenant.Domain.Entities;

namespace Tenant.Persistence.Data.SeedData
{
    internal class CompanyPlanDataSeedConfiguration : IEntityTypeConfiguration<CompanyPlan>
    {
        public void Configure(EntityTypeBuilder<CompanyPlan> builder)
        {
            builder.HasData(
                new
                {
                    Id = Guid.Parse("018da123-abcd-7000-9000-000000000001"),
                    PlanName = "Start-up",
                    PlanPrice = 0,
                    isActive = true,
                    // PlanProperties tablosundaki ilgili ID'yi direkt buraya yapıştırıyoruz:
                    PlanPropertiesId = Guid.Parse("018da123-4567-7000-8000-000000000001")
                },
                new
                {
                    Id = Guid.Parse("018da123-abcd-7000-9000-000000000002"),
                    PlanName = "Business",
                    PlanPrice = 499,
                    isActive = true,
                    PlanPropertiesId = Guid.Parse("018da123-4567-7000-8000-000000000002")
                },
                new
                {
                    Id = Guid.Parse("018da123-abcd-7000-9000-000000000003"),
                    PlanName = "Enterprise",
                    PlanPrice = 1499,
                    isActive = true,
                    PlanPropertiesId = Guid.Parse("018da123-4567-7000-8000-000000000003")
                }
            );
        }
    }
}