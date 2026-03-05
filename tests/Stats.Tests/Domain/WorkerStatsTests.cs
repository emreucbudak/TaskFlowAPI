using Stats.Domain.Entities;

namespace Stats.Tests.Domain;

public class WorkerStatsTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithZeros()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var period = new DateOnly(2026, 3, 15);

        // Act
        var stats = new WorkerStats(userId, period);

        // Assert
        Assert.Equal(0, stats.TotalTasksAssigned);
        Assert.Equal(0, stats.TotalTasksCompleted);
        Assert.Equal(0, stats.TasksCompletedBeforeDeadline);
        Assert.Equal(0, stats.OverdueIncompleteTasksCount);
        Assert.Equal(0, stats.TotalPoints);
    }

    [Fact]
    public void Constructor_ShouldNormalizePeriodToFirstOfMonth()
    {
        // Arrange
        var period = new DateOnly(2026, 3, 15);

        // Act
        var stats = new WorkerStats(Guid.NewGuid(), period);

        // Assert
        Assert.Equal(new DateOnly(2026, 3, 1), stats.Period);
    }

    [Fact]
    public void RecordTaskAssigned_ShouldIncrementCount()
    {
        // Arrange
        var stats = new WorkerStats(Guid.NewGuid(), new DateOnly(2026, 3, 1));

        // Act
        stats.RecordTaskAssigned();

        // Assert
        Assert.Equal(1, stats.TotalTasksAssigned);
    }

    [Fact]
    public void RecordTaskCompleted_BeforeDeadline_ShouldGive20Points()
    {
        // Arrange
        var stats = new WorkerStats(Guid.NewGuid(), new DateOnly(2026, 3, 1));

        // Act
        stats.RecordTaskCompleted(true);

        // Assert
        Assert.Equal(1, stats.TotalTasksCompleted);
        Assert.Equal(1, stats.TasksCompletedBeforeDeadline);
        Assert.Equal(20, stats.TotalPoints);
    }

    [Fact]
    public void RecordTaskCompleted_AfterDeadline_ShouldGive10PointsOnly()
    {
        // Arrange
        var stats = new WorkerStats(Guid.NewGuid(), new DateOnly(2026, 3, 1));

        // Act
        stats.RecordTaskCompleted(false);

        // Assert
        Assert.Equal(1, stats.TotalTasksCompleted);
        Assert.Equal(0, stats.TasksCompletedBeforeDeadline);
        Assert.Equal(10, stats.TotalPoints);
    }

    [Fact]
    public void RecordTaskCompleted_WithDates_EarlyBy5Days_ShouldGive60Points()
    {
        // Arrange
        var stats = new WorkerStats(Guid.NewGuid(), new DateOnly(2026, 3, 1));
        var completedOn = new DateOnly(2026, 3, 5);
        var deadline = new DateOnly(2026, 3, 10);

        // Act
        stats.RecordTaskCompleted(completedOn, deadline);

        // Assert
        Assert.Equal(1, stats.TotalTasksCompleted);
        Assert.Equal(1, stats.TasksCompletedBeforeDeadline);
        Assert.Equal(60, stats.TotalPoints);
    }

    [Fact]
    public void RecordTaskCompleted_WithDates_LateCompletion_ShouldGive10PointsOnly()
    {
        // Arrange
        var stats = new WorkerStats(Guid.NewGuid(), new DateOnly(2026, 3, 1));
        var completedOn = new DateOnly(2026, 3, 12);
        var deadline = new DateOnly(2026, 3, 10);

        // Act
        stats.RecordTaskCompleted(completedOn, deadline);

        // Assert
        Assert.Equal(1, stats.TotalTasksCompleted);
        Assert.Equal(0, stats.TasksCompletedBeforeDeadline);
        Assert.Equal(10, stats.TotalPoints);
    }

    [Fact]
    public void RecordTaskBecameOverdue_ShouldIncrementOverdue()
    {
        // Arrange
        var stats = new WorkerStats(Guid.NewGuid(), new DateOnly(2026, 3, 1));

        // Act
        stats.RecordTaskBecameOverdue();

        // Assert
        Assert.Equal(1, stats.OverdueIncompleteTasksCount);
    }

    [Fact]
    public void RecordOverdueTaskCompleted_ShouldDecrementOverdueAndAddPoints()
    {
        // Arrange
        var stats = new WorkerStats(Guid.NewGuid(), new DateOnly(2026, 3, 1));
        stats.RecordTaskBecameOverdue();

        // Act
        stats.RecordOverdueTaskCompleted();

        // Assert
        Assert.Equal(0, stats.OverdueIncompleteTasksCount);
        Assert.Equal(1, stats.TotalTasksCompleted);
        Assert.Equal(10, stats.TotalPoints);
    }

    [Fact]
    public void RecordOverdueTaskCompleted_WhenNoneOverdue_ShouldNotGoNegative()
    {
        // Arrange
        var stats = new WorkerStats(Guid.NewGuid(), new DateOnly(2026, 3, 1));

        // Act
        stats.RecordOverdueTaskCompleted();

        // Assert
        Assert.Equal(0, stats.OverdueIncompleteTasksCount);
        Assert.Equal(1, stats.TotalTasksCompleted);
        Assert.Equal(10, stats.TotalPoints);
    }

    [Fact]
    public void MultipleOperations_ShouldAccumulateCorrectly()
    {
        // Arrange
        var stats = new WorkerStats(Guid.NewGuid(), new DateOnly(2026, 3, 1));

        // Act
        stats.RecordTaskAssigned();
        stats.RecordTaskAssigned();
        stats.RecordTaskAssigned();
        stats.RecordTaskCompleted(true);
        stats.RecordTaskCompleted(false);
        stats.RecordTaskBecameOverdue();
        stats.RecordOverdueTaskCompleted();

        // Assert
        Assert.Equal(3, stats.TotalTasksAssigned);
        Assert.Equal(3, stats.TotalTasksCompleted);
        Assert.Equal(1, stats.TasksCompletedBeforeDeadline);
        Assert.Equal(0, stats.OverdueIncompleteTasksCount);
        Assert.Equal(40, stats.TotalPoints);
    }
}
