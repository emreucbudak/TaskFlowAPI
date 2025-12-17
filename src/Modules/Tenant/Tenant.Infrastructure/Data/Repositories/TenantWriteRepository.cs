using Microsoft.EntityFrameworkCore;
using Tenant.Application.Repositories;
using Tenant.Domain.Entities;
using Tenant.Infrastructure.Data.TenantDb;

namespace Tenant.Infrastructure.Data.Repositories
{
    public class TenantWriteRepository : ITenantWriteRepository
    {
        private readonly TenantDbContext _context;

        public TenantWriteRepository(TenantDbContext context)
        {
            _context = context;
        }

        public async Task AddPlan(CompanyPlan plan)
        {
            await _context.companyPlans.AddAsync(plan);
        }

        public async Task DeletePlan(CompanyPlan plan)
        {
            _context.companyPlans.Remove(plan);
        }


        public async Task UpdatePlan(CompanyPlan plan)
        {
             _context.companyPlans.Update(plan);
        }
    }
}
