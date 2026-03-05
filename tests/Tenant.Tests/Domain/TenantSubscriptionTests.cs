using TaskFlow.BuildingBlocks.Enums;
using Tenant.Domain.Entities;

namespace Tenant.Tests.Domain;

public class TenantSubscriptionTests
{
    [Fact]
    public void CreateActive_WithValidParams_ShouldCreateActiveSubscription()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var companyPlanId = Guid.NewGuid();
        var usageId = Guid.NewGuid();
        var paymentId = "pay_123";
        var utcNow = new DateTime(2026, 3, 5, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var subscription = TenantSubscription.CreateActive(
            tenantId,
            companyPlanId,
            usageId,
            paymentId,
            utcNow);

        // Assert
        Assert.Equal(SubscriptionStatus.Aktif, subscription.Status);
        Assert.Equal(utcNow.AddMonths(1), subscription.NextBillingDate);
        Assert.Null(subscription.CanceledAt);
        Assert.Equal(paymentId, subscription.PaymentProviderSubscriptionId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateActive_WithEmptyPaymentId_ShouldThrow(string? paymentId)
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var companyPlanId = Guid.NewGuid();
        var usageId = Guid.NewGuid();
        var utcNow = DateTime.UtcNow;

        // Act
        Action act = () => TenantSubscription.CreateActive(
            tenantId,
            companyPlanId,
            usageId,
            paymentId!,
            utcNow);

        // Assert
        Assert.ThrowsAny<ArgumentException>(act);
    }

    [Fact]
    public void Activate_ShouldUpdateAllFields()
    {
        // Arrange
        var oldCompanyPlanId = Guid.NewGuid();
        var subscription = TenantSubscription.CreateActive(
            Guid.NewGuid(),
            oldCompanyPlanId,
            Guid.NewGuid(),
            "old_payment",
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var newCompanyPlanId = Guid.NewGuid();
        var newPaymentId = "new_payment";
        var utcNow = new DateTime(2026, 3, 5, 10, 0, 0, DateTimeKind.Utc);

        // Act
        subscription.Activate(newCompanyPlanId, newPaymentId, utcNow);

        // Assert
        Assert.Equal(newCompanyPlanId, subscription.CompanyPlanId);
        Assert.Equal(newPaymentId, subscription.PaymentProviderSubscriptionId);
        Assert.Equal(SubscriptionStatus.Aktif, subscription.Status);
        Assert.Equal(utcNow, subscription.StartDate);
        Assert.Equal(utcNow.AddMonths(1), subscription.NextBillingDate);
        Assert.Null(subscription.CanceledAt);
    }
}
