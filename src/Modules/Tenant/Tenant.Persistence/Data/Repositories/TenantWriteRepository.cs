using Microsoft.EntityFrameworkCore;
using Tenant.Application.Repositories;
using Tenant.Domain.Entities;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Persistence.Data.Repositories
{
    public sealed class TenantWriteRepository(TenantDbContext context) : ITenantWriteRepository
    {
        private readonly TenantDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public async Task AddPlan(CompanyPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);
            await _context.companyPlans.AddAsync(plan);
        }

        public async Task DeletePlan(CompanyPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);
            var rowsAffected = await _context.companyPlans
                .Where(p => p.Id == plan.Id)
                .ExecuteDeleteAsync();
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Silinecek plan bulunamadý. ID: {plan.Id}");
            }

        }

        public async Task UpdatePlan(CompanyPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);
            var entry = _context.Entry(plan);

            if (entry.State == EntityState.Detached)
            {
                _context.companyPlans.Attach(plan);
                entry.State = EntityState.Modified;
            }

            var affectedRows = await _context.SaveChangesAsync();

            if (affectedRows == 0)
            {
                throw new InvalidOperationException(
                    $"Güncellenecek plan bulunamadý. ID: {plan.Id}");
            }
        }
    }
}
