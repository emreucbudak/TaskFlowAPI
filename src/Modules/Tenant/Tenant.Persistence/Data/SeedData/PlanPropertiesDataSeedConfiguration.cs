using Microsoft.EntityFrameworkCore;
using Tenant.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tenant.Persistence.Data.SeedData
{
    public class PlanPropertiesDataSeedConfiguration : IEntityTypeConfiguration<PlanProperties>
    {
        public void Configure(EntityTypeBuilder<PlanProperties> builder)
        {
            builder.HasData(
                new
                {
                    Id = Guid.Parse("018da123-4567-7000-8000-000000000001"),
                    PeopleAddedLimit = 50,
                    TeamLimit = 10,
                    IndividualTaskLimit = 1000,
                    IsInternalReportingEnabled = true
                },
                new
                {
                    Id = Guid.Parse("018da123-4567-7000-8000-000000000002"),
                    PeopleAddedLimit = 250,
                    TeamLimit = 50,
                    IndividualTaskLimit = 10000,
                    IsInternalReportingEnabled = true
                },
                new
                {
                    Id = Guid.Parse("018da123-4567-7000-8000-000000000003"),
                    PeopleAddedLimit = 10000,
                    TeamLimit = 500,
                    IndividualTaskLimit = 100000,
                    IsInternalReportingEnabled = true
                }
            );
        }
    }
}
