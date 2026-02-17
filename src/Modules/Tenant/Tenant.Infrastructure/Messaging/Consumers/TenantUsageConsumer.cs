using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Contracts.IntegrationEvents;
using Tenant.Infrastructure.Messaging.IntegrationEvents;
using Tenant.Domain.Entities;
using Tenant.Persistence.Data.TenantDb;

namespace Tenant.Infrastructure.Messaging.Consumers;

public sealed class TenantUsageConsumer(
    TenantDbContext dbContext,
    ILogger<TenantUsageConsumer> logger) : ICapSubscribe
{
    private const int MaxRetryCount = 3;

    [CapSubscribe(TenantUsageCapTopics.UserRegistered, Group = "module.tenant")]
    public Task OnUserRegistered(UserRegisteredIntegrationEvent eventData)
    {
        return IncrementUsageWithRetryAsync(eventData.TenantId, usage => usage.IncrementUserCount(), "CurrentUserCount", CancellationToken.None);
    }

    [CapSubscribe(TenantUsageCapTopics.GroupChatCreated, Group = "module.tenant")]
    public Task OnGroupChatCreated(GroupChatCreatedIntegrationEvent eventData)
    {
        return IncrementUsageWithRetryAsync(eventData.TenantId, usage => usage.IncrementGroupCount(), "CurrentGroupCount", CancellationToken.None);
    }

    [CapSubscribe(TenantUsageCapTopics.IndividualTaskCreated, Group = "module.tenant")]
    public Task OnIndividualTaskCreated(IndividualTaskCreatedIntegrationEvent eventData)
    {
        return IncrementUsageWithRetryAsync(eventData.TenantId, usage => usage.IncrementIndividualTaskCount(), "CurrentIndividualTaskCount", CancellationToken.None);
    }

    private async Task IncrementUsageWithRetryAsync(
        Guid tenantId,
        Action<TenantUsage> incrementAction,
        string metricName,
        CancellationToken cancellationToken)
    {
        // RowVersion is checked by EF as a concurrency token.
        // If two consumers update the same tenant concurrently, one save fails and is retried.
        for (var attempt = 1; attempt <= MaxRetryCount; attempt++)
        {
            try
            {
                var usage = await dbContext.tenantUsages.SingleOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);
                if (usage is null)
                {
                    usage = new TenantUsage(tenantId);
                    dbContext.tenantUsages.Add(usage);
                }

                incrementAction(usage);
                await dbContext.SaveChangesAsync(cancellationToken);
                return;
            }
            catch (DbUpdateConcurrencyException ex) when (attempt < MaxRetryCount)
            {
                logger.LogWarning(
                    ex,
                    "TenantUsage optimistic concurrency conflict. TenantId={TenantId}, Metric={Metric}, Attempt={Attempt}",
                    tenantId,
                    metricName,
                    attempt);

                dbContext.ChangeTracker.Clear();
            }
            catch (DbUpdateException ex) when (attempt < MaxRetryCount)
            {
                logger.LogWarning(
                    ex,
                    "TenantUsage write conflict. TenantId={TenantId}, Metric={Metric}, Attempt={Attempt}",
                    tenantId,
                    metricName,
                    attempt);

                dbContext.ChangeTracker.Clear();
            }
        }

        throw new DbUpdateConcurrencyException(
            $"TenantUsage update failed after {MaxRetryCount} retries. TenantId={tenantId}, Metric={metricName}");
    }
}
