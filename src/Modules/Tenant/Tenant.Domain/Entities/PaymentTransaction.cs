using TaskFlow.BuildingBlocks.Common;

namespace Tenant.Domain.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        public byte[] RowVersion { get; private set; }
        public Guid TenantId { get; private set; }

        public Guid TenantSubscriptionId { get; private set; }

        public string ExternalTransactionId { get; private set; }

        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = "TRY";

        public bool IsSuccess { get; private set; }
        public string? FailureMessage { get; private set; }

        public DateTime ProcessedAt { get; private set; }
    }
}
