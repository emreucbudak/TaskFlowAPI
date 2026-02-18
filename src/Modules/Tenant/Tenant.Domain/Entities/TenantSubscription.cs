using System.ComponentModel.DataAnnotations;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Enums;

namespace Tenant.Domain.Entities
{
    public class TenantSubscription : BaseEntity
    {
        public Guid TenantId { get; private set; }

        public Guid CompanyPlanId { get; private set; }
        public  CompanyPlan CompanyPlan { get; private set; }

        public string PaymentProviderSubscriptionId { get; private set; }

        public SubscriptionStatus Status { get; private set; }
        public Guid TenantUsageId { get; private set; }
        public TenantUsage TenantUsage { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime NextBillingDate { get; private set; }
        public DateTime? CanceledAt { get; private set; }
        [Timestamp]
        public byte[] RowVersion { get; private set; }
    }
}
