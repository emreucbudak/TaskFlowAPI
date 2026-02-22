using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskFlow.BuildingBlocks.Enums;
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
                throw new InvalidOperationException($"Plan not found for delete. Id: {plan.Id}");
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
                throw new InvalidOperationException($"Plan not found for update. Id: {plan.Id}");
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TenantUsage> GetOrCreateTenantUsage(Guid tenantId, CancellationToken cancellationToken)
        {
            var tenantUsage = await _context.tenantUsages
                .SingleOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

            if (tenantUsage is null)
            {
                tenantUsage = new TenantUsage(tenantId);
                _context.tenantUsages.Add(tenantUsage);

                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException ex) when (IsTenantUsageUniqueViolation(ex))
                {
                    _context.Entry(tenantUsage).State = EntityState.Detached;

                    var existingTenantUsage = await _context.tenantUsages
                        .SingleOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

                    if (existingTenantUsage is not null)
                    {
                        return existingTenantUsage;
                    }

                    throw;
                }
            }

            return tenantUsage;
        }

        public async Task<TenantSubscription?> GetTenantSubscription(Guid tenantId, CancellationToken cancellationToken)
        {
            return await _context.tenantSubscriptions
                .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
        }

        public Task AddTenantSubscription(TenantSubscription tenantSubscription, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(tenantSubscription);
            _context.tenantSubscriptions.Add(tenantSubscription);
            return Task.CompletedTask;
        }

        public async Task UpdateTenantSubscription(
            Guid tenantId,
            Guid companyPlanId,
            string paymentProviderSubscriptionId,
            DateTime utcNow,
            CancellationToken cancellationToken)
        {
            var affectedRows = await _context.tenantSubscriptions
                .Where(x => x.TenantId == tenantId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.CompanyPlanId, companyPlanId)
                    .SetProperty(x => x.PaymentProviderSubscriptionId, paymentProviderSubscriptionId)
                    .SetProperty(x => x.Status, SubscriptionStatus.Aktif)
                    .SetProperty(x => x.StartDate, utcNow)
                    .SetProperty(x => x.NextBillingDate, utcNow.AddMonths(1))
                    .SetProperty(x => x.CanceledAt, (DateTime?)null)
                    .SetProperty(x => x.RowVersion, Guid.NewGuid().ToByteArray()),
                    cancellationToken);

            if (affectedRows == 0)
            {
                throw new InvalidOperationException($"Tenant subscription not found for update. TenantId: {tenantId}");
            }
        }

        private static bool IsTenantUsageUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException postgresException
                && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
                && string.Equals(postgresException.ConstraintName, "IX_tenantUsages_TenantId", StringComparison.OrdinalIgnoreCase);
        }
    }
}
