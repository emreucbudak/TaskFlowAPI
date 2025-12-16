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

        public Task<List<CompanyPlan>> GetAllPlans(bool trackChanges)
        {
            IQueryable<CompanyPlan> query = _context.companyPlans;
            if (!trackChanges)
                query = query.AsNoTracking();
            return query.ToListAsync();
        }

        public async Task<CompanyPlan> GetPlan(Guid id,bool trackChanges)
        {
            IQueryable<CompanyPlan> query = _context.companyPlans;
            if (!trackChanges)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(p => p.Id == id);

        }

        public async Task UpdatePlan(CompanyPlan plan)
        {
             _context.companyPlans.Update(plan);

        }
    }
}
