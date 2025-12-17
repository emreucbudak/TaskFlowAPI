using Tenant.Domain.Entities;

namespace Tenant.Application.Repositories
{
    public interface ITenantWriteRepository
    {
        Task AddPlan(CompanyPlan plan);
        Task DeletePlan (CompanyPlan plan);
        Task UpdatePlan (CompanyPlan plan);

    }
}
