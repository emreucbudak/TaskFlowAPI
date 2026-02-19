using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tenant.Domain.Entities;

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
                    PeopleAddedLimit = 5,
                    TeamLimit = 1,
                    IndividualTaskLimit = 100,
                    IsInternalReportingEnabled = true
                },

       
                new
                {
                    Id = Guid.Parse("018da123-4567-7000-8000-000000000002"),
                    PeopleAddedLimit = 25,
                    TeamLimit = 5,
                    IndividualTaskLimit = 1000,
                    IsInternalReportingEnabled = true
                },

                new
                {
                    Id = Guid.Parse("018da123-4567-7000-8000-000000000003"),
                    PeopleAddedLimit = 1000,
                    TeamLimit = 50,
                    IndividualTaskLimit = 10000,
                    IsInternalReportingEnabled = true
                }
            );
        }
    }
}