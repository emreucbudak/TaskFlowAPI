using System.ComponentModel.DataAnnotations;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Enums;

namespace Tenant.Domain.Entities
{
    public class TenantSubscription : BaseEntity
    {
        protected TenantSubscription() { }

        public Guid TenantId { get; private set; }
        public Guid CompanyPlanId { get; private set; }
        public CompanyPlan CompanyPlan { get; private set; }
        public string PaymentProviderSubscriptionId { get; private set; }
        public SubscriptionStatus Status { get; private set; }
        public Guid TenantUsageId { get; private set; }
        public TenantUsage TenantUsage { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime NextBillingDate { get; private set; }
        public DateTime? CanceledAt { get; private set; }

        [Timestamp]
        public byte[] RowVersion { get; private set; }

        public static TenantSubscription CreateActive(
            Guid tenantId,
            Guid companyPlanId,
            Guid tenantUsageId,
            string paymentProviderSubscriptionId,
            DateTime utcNow)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(paymentProviderSubscriptionId);

            return new TenantSubscription
            {
                TenantId = tenantId,
                CompanyPlanId = companyPlanId,
                TenantUsageId = tenantUsageId,
                PaymentProviderSubscriptionId = paymentProviderSubscriptionId,
                Status = SubscriptionStatus.Aktif,
                StartDate = utcNow,
                NextBillingDate = utcNow.AddMonths(1),
                CanceledAt = null,
                RowVersion = CreateNextVersion()
            };
        }

        public void Activate(Guid companyPlanId, string paymentProviderSubscriptionId, DateTime utcNow)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(paymentProviderSubscriptionId);

            CompanyPlanId = companyPlanId;
            PaymentProviderSubscriptionId = paymentProviderSubscriptionId;
            Status = SubscriptionStatus.Aktif;
            StartDate = utcNow;
            NextBillingDate = utcNow.AddMonths(1);
            CanceledAt = null;
            RowVersion = CreateNextVersion();
        }

        private static byte[] CreateNextVersion()
        {
            return Guid.NewGuid().ToByteArray();
        }
    }
}
