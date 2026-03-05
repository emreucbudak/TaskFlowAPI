using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Tests.Domain;

public class IndividualTasksTests
{
    [Fact]
    public void Constructor_WithValidParams_ShouldCreate()
    {
        // Arrange
        var assignedUserId = Guid.NewGuid();
        var title = "Task Title";
        var description = "Task Description";
        var deadline = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        const int priorityId = 2;

        // Act
        var task = new IndividualTasks(assignedUserId, title, description, deadline, priorityId);

        // Assert
        Assert.Equal(assignedUserId, task.AssignedUserId);
        Assert.Equal(title, task.TaskTitle);
        Assert.Equal(description, task.Description);
        Assert.Equal(deadline, task.Deadline);
        Assert.Equal(priorityId, task.TaskPriorityCategoryId);
    }

    [Fact]
    public void Update_ShouldModifyAllFields()
    {
        // Arrange
        var task = new IndividualTasks(
            Guid.NewGuid(),
            "Old Title",
            "Old Description",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            1);

        var newTitle = "New Title";
        var newDescription = "New Description";
        var newDeadline = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        const int newPriority = 3;

        // Act
        task.Update(newTitle, newDescription, newDeadline, newPriority);

        // Assert
        Assert.Equal(newTitle, task.TaskTitle);
        Assert.Equal(newDescription, task.Description);
        Assert.Equal(newDeadline, task.Deadline);
        Assert.Equal(newPriority, task.TaskPriorityCategoryId);
    }
}
