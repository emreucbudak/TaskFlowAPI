using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Tests.Domain;

public class SubtaskTests
{
    [Fact]
    public void Constructor_WithValidParams_ShouldCreate()
    {
        // Arrange
        var description = "Subtask Description";
        var assignedUserId = Guid.NewGuid();
        const int statusId = 1;
        var title = "Subtask Title";
        var taskId = Guid.NewGuid();

        // Act
        var subtask = new Subtask(description, assignedUserId, statusId, title, taskId);

        // Assert
        Assert.Equal(description, subtask.Description);
        Assert.Equal(assignedUserId, subtask.AssignedUserId);
        Assert.Equal(statusId, subtask.TaskStatusId);
        Assert.Equal(title, subtask.TaskTitle);
        Assert.Equal(taskId, subtask.TaskId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyTitle_ShouldThrow(string? title)
    {
        // Arrange
        var description = "Subtask Description";
        var assignedUserId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        // Act
        Action act = () => new Subtask(description, assignedUserId, 1, title!, taskId);

        // Assert
        Assert.Throws<TaskDomainException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyDescription_ShouldThrow()
    {
        // Arrange
        var assignedUserId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        // Act
        Action act = () => new Subtask(string.Empty, assignedUserId, 1, "Title", taskId);

        // Assert
        Assert.Throws<TaskDomainException>(act);
    }

    [Fact]
    public void AddAnswer_ShouldAddToCollection()
    {
        // Arrange
        var subtask = CreateSubtask();
        var answer = new SubTaskAnswer("Answer 1", Guid.NewGuid());

        // Act
        subtask.AddAnswer(answer);

        // Assert
        Assert.Single(subtask.subTaskAnswers);
        Assert.Equal(answer.Id, subtask.subTaskAnswers.Single().Id);
    }

    [Fact]
    public void UpdateTaskStatus_ShouldChange()
    {
        // Arrange
        var subtask = CreateSubtask();

        // Act
        subtask.UpdateTaskStatus(4);

        // Assert
        Assert.Equal(4, subtask.TaskStatusId);
    }

    [Fact]
    public void RemoveSubTaskAnswer_WithInvalidId_ShouldThrow()
    {
        // Arrange
        var subtask = CreateSubtask();

        // Act
        Action act = () => subtask.RemoveSubTaskAnswer(Guid.NewGuid());

        // Assert
        Assert.Throws<TaskDomainException>(act);
    }

    [Fact]
    public void UpdateSubTaskAnswer_ShouldUpdateText()
    {
        // Arrange
        var subtask = CreateSubtask();
        var answer = new SubTaskAnswer("Old Answer", Guid.NewGuid());
        subtask.AddAnswer(answer);

        // Act
        subtask.UpdateSubTaskAnswer("New Answer", answer.Id);

        // Assert
        Assert.Equal("New Answer", answer.AnswerText);
    }

    private static Subtask CreateSubtask()
    {
        return new Subtask(
            "Subtask Description",
            Guid.NewGuid(),
            1,
            "Subtask Title",
            Guid.NewGuid());
    }
}
