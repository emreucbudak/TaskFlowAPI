using Tenant.Domain.Entities;

namespace Tenant.Tests.Domain;

public class TenantUsageTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithZeroCounts()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var usage = new TenantUsage(tenantId);

        // Assert
        Assert.Equal(0, usage.CurrentUserCount);
        Assert.Equal(0, usage.CurrentTaskCount);
        Assert.Equal(0, usage.CurrentGroupCount);
        Assert.Equal(0, usage.CurrentIndividualTaskCount);
        Assert.NotNull(usage.RowVersion);
        Assert.NotEmpty(usage.RowVersion);
    }

    [Fact]
    public void IncrementUserCount_ShouldIncrease()
    {
        // Arrange
        var usage = new TenantUsage(Guid.NewGuid());

        // Act
        usage.IncrementUserCount();

        // Assert
        Assert.Equal(1, usage.CurrentUserCount);
    }

    [Fact]
    public void DecrementUserCount_ShouldDecrease()
    {
        // Arrange
        var usage = new TenantUsage(Guid.NewGuid());
        usage.IncrementUserCount();

        // Act
        usage.DecrementUserCount();

        // Assert
        Assert.Equal(0, usage.CurrentUserCount);
    }

    [Fact]
    public void DecrementUserCount_WhenZero_ShouldNotGoNegative()
    {
        // Arrange
        var usage = new TenantUsage(Guid.NewGuid());

        // Act
        usage.DecrementUserCount();

        // Assert
        Assert.Equal(0, usage.CurrentUserCount);
    }

    [Fact]
    public void IncrementTaskCount_ShouldIncrease()
    {
        // Arrange
        var usage = new TenantUsage(Guid.NewGuid());

        // Act
        usage.IncrementTaskCount();

        // Assert
        Assert.Equal(1, usage.CurrentTaskCount);
    }

    [Fact]
    public void IncrementGroupCount_ShouldIncrease()
    {
        // Arrange
        var usage = new TenantUsage(Guid.NewGuid());

        // Act
        usage.IncrementGroupCount();

        // Assert
        Assert.Equal(1, usage.CurrentGroupCount);
    }

    [Fact]
    public void IncrementIndividualTaskCount_ShouldIncrease()
    {
        // Arrange
        var usage = new TenantUsage(Guid.NewGuid());

        // Act
        usage.IncrementIndividualTaskCount();

        // Assert
        Assert.Equal(1, usage.CurrentIndividualTaskCount);
    }

    [Fact]
    public void RowVersion_ShouldChangeOnEveryUpdate()
    {
        // Arrange
        var usage = new TenantUsage(Guid.NewGuid());
        var initialVersion = usage.RowVersion.ToArray();

        // Act
        usage.IncrementUserCount();
        var secondVersion = usage.RowVersion.ToArray();
        usage.IncrementTaskCount();
        var thirdVersion = usage.RowVersion.ToArray();

        // Assert
        Assert.False(initialVersion.SequenceEqual(secondVersion));
        Assert.False(secondVersion.SequenceEqual(thirdVersion));
    }
}
