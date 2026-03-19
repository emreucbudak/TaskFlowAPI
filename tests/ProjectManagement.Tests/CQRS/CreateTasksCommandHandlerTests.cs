using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using ProjectManagement.Application.Features.CQRS.Tasks.Command.Create;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Application.UnitOfWork;
using DomainTask = ProjectManagement.Domain.Entities.Task;

namespace ProjectManagement.Tests.CQRS;

public class CreateTasksCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithPastDeadline_ShouldThrowArgumentException()
    {
        var writeRepositoryMock = new Mock<IProjectManagementWriteRepository>();
        var unitOfWorkMock = new Mock<IProjectManagementCapUnitOfWork>();
        var capPublisherMock = new Mock<ICapPublisher>();

        var handler = new CreateTasksCommandHandler(
            unitOfWorkMock.Object,
            writeRepositoryMock.Object,
            capPublisherMock.Object);

        var request = new CreateTasksCommandRequest(
            "Task",
            "Description",
            DateTime.UtcNow.AddDays(-1),
            2);

        Func<Task> act = () => handler.Handle(request, CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(act);
        writeRepositoryMock.Verify(x => x.AddTask(It.IsAny<DomainTask>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithoutPriority_ShouldThrowArgumentException()
    {
        var writeRepositoryMock = new Mock<IProjectManagementWriteRepository>();
        var unitOfWorkMock = new Mock<IProjectManagementCapUnitOfWork>();
        var capPublisherMock = new Mock<ICapPublisher>();

        var handler = new CreateTasksCommandHandler(
            unitOfWorkMock.Object,
            writeRepositoryMock.Object,
            capPublisherMock.Object);

        var request = new CreateTasksCommandRequest(
            "Task",
            "Description",
            DateTime.UtcNow.AddDays(5),
            0);

        Func<Task> act = () => handler.Handle(request, CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(act);
        writeRepositoryMock.Verify(x => x.AddTask(It.IsAny<DomainTask>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCallAddTaskAndPublishAndCommit()
    {
        var writeRepositoryMock = new Mock<IProjectManagementWriteRepository>();
        var unitOfWorkMock = new Mock<IProjectManagementCapUnitOfWork>();
        var capPublisherMock = new Mock<ICapPublisher>();
        var transactionMock = new Mock<IDbContextTransaction>();

        unitOfWorkMock
            .Setup(x => x.BeginTransaction(capPublisherMock.Object, false))
            .Returns(transactionMock.Object);

        writeRepositoryMock
            .Setup(x => x.AddTask(It.IsAny<DomainTask>()))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        capPublisherMock
            .Setup(x => x.PublishDelayAsync(
                It.IsAny<TimeSpan>(),
                "TaskCreated",
                It.IsAny<TaskCreatedIntegrationEvent>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        transactionMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateTasksCommandHandler(
            unitOfWorkMock.Object,
            writeRepositoryMock.Object,
            capPublisherMock.Object);

        var request = new CreateTasksCommandRequest(
            "Task",
            "Description",
            DateTime.UtcNow.AddDays(5),
            2);

        await handler.Handle(request, CancellationToken.None);

        writeRepositoryMock.Verify(
            x => x.AddTask(It.Is<DomainTask>(task => task.TaskPriorityCategoryId == 2)),
            Times.Once);
        capPublisherMock.Verify(x => x.PublishDelayAsync(
            It.IsAny<TimeSpan>(),
            "TaskCreated",
            It.IsAny<TaskCreatedIntegrationEvent>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenWriteRepoThrows_ShouldRollbackTransaction()
    {
        var writeRepositoryMock = new Mock<IProjectManagementWriteRepository>();
        var unitOfWorkMock = new Mock<IProjectManagementCapUnitOfWork>();
        var capPublisherMock = new Mock<ICapPublisher>();
        var transactionMock = new Mock<IDbContextTransaction>();

        unitOfWorkMock
            .Setup(x => x.BeginTransaction(capPublisherMock.Object, false))
            .Returns(transactionMock.Object);

        writeRepositoryMock
            .Setup(x => x.AddTask(It.IsAny<DomainTask>()))
            .ThrowsAsync(new InvalidOperationException("db failure"));

        transactionMock
            .Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateTasksCommandHandler(
            unitOfWorkMock.Object,
            writeRepositoryMock.Object,
            capPublisherMock.Object);

        var request = new CreateTasksCommandRequest(
            "Task",
            "Description",
            DateTime.UtcNow.AddDays(5),
            2);

        Func<Task> act = () => handler.Handle(request, CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(act);
        transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
