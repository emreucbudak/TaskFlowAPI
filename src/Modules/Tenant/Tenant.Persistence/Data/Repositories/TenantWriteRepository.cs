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
        private const int MaxConcurrencyRetryCount = 5;
        private readonly TenantDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        public void AddPlan(CompanyPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);
            _context.CompanyPlans.Add(plan);
        }

        public async Task DeletePlan(CompanyPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);
            var rowsAffected = await _context.CompanyPlans
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
                _context.CompanyPlans.Attach(plan);
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
            var tenantUsage = await _context.TenantUsages
                .SingleOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

            if (tenantUsage is null)
            {
                tenantUsage = new TenantUsage(tenantId);
                _context.TenantUsages.Add(tenantUsage);

                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException ex) when (IsTenantUsageUniqueViolation(ex))
                {
                    _context.Entry(tenantUsage).State = EntityState.Detached;

                    var existingTenantUsage = await _context.TenantUsages
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

        public async Task<bool> TryReserveLimitSlot(Guid tenantId, LimitType limitType, int limit, CancellationToken cancellationToken)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
            }

            if (limit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), "Limit cannot be negative.");
            }

            for (var attempt = 1; attempt <= MaxConcurrencyRetryCount; attempt++)
            {
                var tenantUsage = await GetOrCreateTenantUsage(tenantId, cancellationToken);
                var currentUsage = GetCurrentUsage(tenantUsage, limitType);

                if (currentUsage >= limit)
                {
                    return false;
                }

                IncrementUsage(tenantUsage, limitType);

                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return true;
                }
                catch (DbUpdateConcurrencyException) when (attempt < MaxConcurrencyRetryCount)
                {
                    _context.ChangeTracker.Clear();
                }
            }

            throw new DbUpdateConcurrencyException(
                $"Failed to reserve limit slot after {MaxConcurrencyRetryCount} retries. TenantId={tenantId}, LimitType={limitType}");
        }

        public async Task ReleaseReservedLimitSlot(Guid tenantId, LimitType limitType, CancellationToken cancellationToken)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
            }

            for (var attempt = 1; attempt <= MaxConcurrencyRetryCount; attempt++)
            {
                var tenantUsage = await _context.TenantUsages
                    .SingleOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

                if (tenantUsage is null)
                {
                    return;
                }

                if (GetCurrentUsage(tenantUsage, limitType) <= 0)
                {
                    return;
                }

                DecrementUsage(tenantUsage, limitType);

                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    return;
                }
                catch (DbUpdateConcurrencyException) when (attempt < MaxConcurrencyRetryCount)
                {
                    _context.ChangeTracker.Clear();
                }
            }

            throw new DbUpdateConcurrencyException(
                $"Failed to release reserved limit slot after {MaxConcurrencyRetryCount} retries. TenantId={tenantId}, LimitType={limitType}");
        }

        public Task<TenantSubscription?> GetTenantSubscription(Guid tenantId, CancellationToken cancellationToken)
            => _context.TenantSubscriptions
                .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

        public void AddTenantSubscription(TenantSubscription tenantSubscription)
        {
            ArgumentNullException.ThrowIfNull(tenantSubscription);
            _context.TenantSubscriptions.Add(tenantSubscription);
        }

        public async Task UpdateTenantSubscription(
            Guid tenantId,
            Guid companyPlanId,
            string paymentProviderSubscriptionId,
            DateTime utcNow,
            CancellationToken cancellationToken)
        {
            var affectedRows = await _context.TenantSubscriptions
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

        public async Task ResetTenantUsageCounters(Guid tenantId, CancellationToken cancellationToken)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
            }

            var affectedRows = await _context.TenantUsages
                .Where(x => x.TenantId == tenantId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.CurrentUserCount, 0)
                    .SetProperty(x => x.CurrentTaskCount, 0)
                    .SetProperty(x => x.CurrentGroupCount, 0)
                    .SetProperty(x => x.CurrentIndividualTaskCount, 0)
                    .SetProperty(x => x.RowVersion, Guid.NewGuid().ToByteArray()),
                    cancellationToken);

            if (affectedRows > 0)
            {
                return;
            }

            _context.TenantUsages.Add(new TenantUsage(tenantId));
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static bool IsTenantUsageUniqueViolation(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException postgresException
                && postgresException.SqlState == PostgresErrorCodes.UniqueViolation
                && string.Equals(postgresException.ConstraintName, "IX_tenantUsages_TenantId", StringComparison.OrdinalIgnoreCase);
        }

        private static int GetCurrentUsage(TenantUsage tenantUsage, LimitType limitType)
        {
            return limitType switch
            {
                LimitType.PeopleAdded => tenantUsage.CurrentUserCount,
                LimitType.TeamLimit => tenantUsage.CurrentGroupCount,
                LimitType.IndividualTask => tenantUsage.CurrentIndividualTaskCount,
                _ => throw new NotSupportedException($"Unsupported limit type: {limitType}")
            };
        }

        private static void IncrementUsage(TenantUsage tenantUsage, LimitType limitType)
        {
            switch (limitType)
            {
                case LimitType.PeopleAdded:
                    tenantUsage.IncrementUserCount();
                    break;
                case LimitType.TeamLimit:
                    tenantUsage.IncrementGroupCount();
                    break;
                case LimitType.IndividualTask:
                    tenantUsage.IncrementIndividualTaskCount();
                    break;
                default:
                    throw new NotSupportedException($"Unsupported limit type: {limitType}");
            }
        }

        private static void DecrementUsage(TenantUsage tenantUsage, LimitType limitType)
        {
            switch (limitType)
            {
                case LimitType.PeopleAdded:
                    tenantUsage.DecrementUserCount();
                    break;
                case LimitType.TeamLimit:
                    tenantUsage.DecrementGroupCount();
                    break;
                case LimitType.IndividualTask:
                    tenantUsage.DecrementIndividualTaskCount();
                    break;
                default:
                    throw new NotSupportedException($"Unsupported limit type: {limitType}");
            }
        }
    }
}
