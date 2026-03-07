using Moq;
using ProjectManagement.Application.Features.CQRS.Tasks.Queries.GetByAssignedUsers;
using ProjectManagement.Application.Repositories;
using DomainTask = ProjectManagement.Domain.Entities.Task;

namespace ProjectManagement.Tests.CQRS;

public class GetGroupTasksByAssignedUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithEmptyAssignedUserList_ShouldReturnEmptyPage()
    {
        var repositoryMock = new Mock<IProjectManagementReadRepository>();
        var handler = new GetGroupTasksByAssignedUsersQueryHandler(repositoryMock.Object);

        var request = new GetGroupTasksByAssignedUsersQueryRequest
        {
            AssignedUserIds = [],
            PageNumber = 1,
            PageSize = 20
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        repositoryMock.Verify(
            repository => repository.GetTasksByAssignedUserIds(
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithMatchingTasks_ShouldMapPagedGroupTasks()
    {
        var assignedUserId = Guid.NewGuid();
        var task = new DomainTask(
            "Sprint Plan",
            "Takim planlamasi",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DateOnly.FromDateTime(DateTime.UtcNow));

        task.AddSubTask("Ilk alt gorev", assignedUserId, "Dokumani hazirla", task.Id);

        var repositoryMock = new Mock<IProjectManagementReadRepository>();
        repositoryMock
            .Setup(repository => repository.GetTasksByAssignedUserIds(
                false,
                2,
                10,
                It.Is<IReadOnlyCollection<Guid>>(ids => ids.Count == 1 && ids.Contains(assignedUserId)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<DomainTask> { task }, 1));

        var handler = new GetGroupTasksByAssignedUsersQueryHandler(repositoryMock.Object);

        var request = new GetGroupTasksByAssignedUsersQueryRequest
        {
            AssignedUserIds = [assignedUserId],
            PageNumber = 2,
            PageSize = 10
        };

        var result = await handler.Handle(request, CancellationToken.None);

        var responseItem = Assert.Single(result.Items);
        Assert.Equal("Sprint Plan", responseItem.TaskName);
        Assert.Equal("Takim planlamasi", responseItem.Description);
        Assert.Equal("Grup", responseItem.CategoryName);
        Assert.Equal("Belirtilmedi", responseItem.TaskPriorityName);
        Assert.Single(responseItem.SubTasks);
        Assert.Equal(assignedUserId, responseItem.SubTasks[0].AssignedUserId);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
    }
}