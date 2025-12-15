using Microsoft.EntityFrameworkCore;
using Tenant.Domain.Entities;

namespace Tenant.Application.Repositories
{
    public interface ITenantReadRepository
    {
       DbSet<CompanyPlan> Companies {  get; }
       DbSet<PlanProperties> PlanProperties { get; }
    }
}
