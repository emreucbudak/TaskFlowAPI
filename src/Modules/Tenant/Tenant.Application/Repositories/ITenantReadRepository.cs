using Microsoft.EntityFrameworkCore;
using Tenant.Domain.Entities;

namespace Tenant.Application.Repositories
{
    public interface ITenantReadRepository
    {
        Task<CompanyPlan> GetPlan(Guid id, bool trackChanges);
        Task<List<CompanyPlan>> GetAllPlans(bool trackChanges);
    }
}
