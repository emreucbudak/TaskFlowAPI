using Microsoft.EntityFrameworkCore;
using Tenant.Application.Repositories;
using Tenant.Domain.Entities;
using Tenant.Infrastructure.Data.TenantDb;

namespace Tenant.Infrastructure.Data.Repositories
{
    public class TenantReadRepository : ITenantReadRepository
    {
        private readonly TenantDbContext _context;

        public TenantReadRepository(TenantDbContext context)
        {
            _context = context;
        }

        public Task<List<CompanyPlan>> GetAllPlans(bool trackChanges)
        {
            IQueryable<CompanyPlan> query = _context.companyPlans;
            if (!trackChanges)
                query = query.AsNoTracking();
            return query.ToListAsync();
        }

        public async Task<CompanyPlan> GetPlan(Guid id, bool trackChanges)
        {
            IQueryable<CompanyPlan> query = _context.companyPlans;
            if (!trackChanges)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(p => p.Id == id);

        }
    }
}
