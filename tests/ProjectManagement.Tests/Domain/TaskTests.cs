using DomainTask = ProjectManagement.Domain.Entities.Task;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Tests.Domain;

public class TaskTests
{
    [Fact]
    public void Constructor_WithValidParams_ShouldCreateTask()
    {
        // Arrange
        var taskName = "Task A";
        var description = "Description A";
        var deadline = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var task = new DomainTask(taskName, description, deadline, createdDate);

        // Assert
        Assert.Equal(taskName, task.TaskName);
        Assert.Equal(description, task.Description);
        Assert.Equal(1, task.TaskStatusId);
        Assert.Equal(deadline, task.DeadlineTime);
        Assert.Equal(createdDate, task.CreatedDate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrEmptyTaskName_ShouldThrow(string? taskName)
    {
        // Arrange
        var description = "Description";
        var deadline = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        Action act = () => new DomainTask(taskName!, description, deadline, createdDate);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyDescription_ShouldThrow()
    {
        // Arrange
        var deadline = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));
        var createdDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        Action act = () => new DomainTask("Task A", string.Empty, deadline, createdDate);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void UpdateTaskStatus_ShouldChangeStatus()
    {
        // Arrange
        var task = CreateTask();

        // Act
        task.UpdateTaskStatus(3);

        // Assert
        Assert.Equal(3, task.TaskStatusId);
    }

    [Fact]
    public void UpdateTaskName_WithValidName_ShouldUpdate()
    {
        // Arrange
        var task = CreateTask();
        var newName = "Updated Name";

        // Act
        task.UpdateTaskName(newName);

        // Assert
        Assert.Equal(newName, task.TaskName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateTaskName_WithNullOrEmpty_ShouldThrow(string? name)
    {
        // Arrange
        var task = CreateTask();

        // Act
        Action act = () => task.UpdateTaskName(name);

        // Assert
        Assert.Throws<TaskDomainException>(act);
    }

    [Fact]
    public void AddSubTask_WhenStatusIsCompleted_ShouldThrowInvalidOperation()
    {
        // Arrange
        var task = CreateTask();
        task.UpdateTaskStatus(2);

        // Act
        Action act = () => task.AddSubTask("Sub Desc", Guid.NewGuid(), "Sub Title", Guid.NewGuid());

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void AddSubTask_WhenStatusIsActive_ShouldAddSubTask()
    {
        // Arrange
        var task = CreateTask();

        // Act
        var subTask = task.AddSubTask("Sub Desc", Guid.NewGuid(), "Sub Title", Guid.NewGuid());

        // Assert
        Assert.Single(task.subtask);
        Assert.Equal(subTask.Id, task.subtask.Single().Id);
    }

    [Fact]
    public void RemoveSubTask_WithExistingId_ShouldRemove()
    {
        // Arrange
        var task = CreateTask();
        var subTask = task.AddSubTask("Sub Desc", Guid.NewGuid(), "Sub Title", Guid.NewGuid());

        // Act
        task.RemoveSubTask(subTask.Id);

        // Assert
        Assert.Empty(task.subtask);
    }

    [Fact]
    public void RemoveSubTask_WithNonExistingId_ShouldThrow()
    {
        // Arrange
        var task = CreateTask();

        // Act
        Action act = () => task.RemoveSubTask(Guid.NewGuid());

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void UpdateDeadlineTime_WithPastDate_ShouldThrow()
    {
        // Arrange
        var task = CreateTask();
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        // Act
        Action act = () => task.UpdateDeadlineTime(pastDate);

        // Assert
        Assert.Throws<TaskDomainException>(act);
    }

    [Fact]
    public void UpdateDeadlineTime_WithFutureDate_ShouldUpdate()
    {
        // Arrange
        var task = CreateTask();
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));

        // Act
        task.UpdateDeadlineTime(futureDate);

        // Assert
        Assert.Equal(futureDate, task.DeadlineTime);
    }

    [Fact]
    public void UpdateDeadlineTime_WithNull_ShouldNotChange()
    {
        // Arrange
        var task = CreateTask();
        var originalDeadline = task.DeadlineTime;

        // Act
        task.UpdateDeadlineTime(null);

        // Assert
        Assert.Equal(originalDeadline, task.DeadlineTime);
    }

    [Fact]
    public void GetSubtask_WithValidId_ShouldReturn()
    {
        // Arrange
        var task = CreateTask();
        var subTask = task.AddSubTask("Sub Desc", Guid.NewGuid(), "Sub Title", Guid.NewGuid());

        // Act
        var result = task.GetSubtask(subTask.Id);

        // Assert
        Assert.Equal(subTask.Id, result.Id);
    }

    [Fact]
    public void GetSubtask_WithInvalidId_ShouldThrow()
    {
        // Arrange
        var task = CreateTask();

        // Act
        Action act = () => task.GetSubtask(Guid.NewGuid());

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    private static DomainTask CreateTask()
    {
        return new DomainTask(
            "Task A",
            "Description A",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
