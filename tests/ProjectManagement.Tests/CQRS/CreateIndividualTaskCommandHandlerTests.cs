using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Create;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;
using IndividualTask = ProjectManagement.Domain.Entities.IndividualTasks;

namespace ProjectManagement.Tests.CQRS;

public class CreateIndividualTaskCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRequest_ShouldCallAddAndPublishAndCommit()
    {
        // Arrange
        var writeRepositoryMock = new Mock<IProjectManagementWriteRepository>();
        var unitOfWorkMock = new Mock<ICapUnitOfWork>();
        var capPublisherMock = new Mock<ICapPublisher>();
        var transactionMock = new Mock<IDbContextTransaction>();

        unitOfWorkMock
            .Setup(x => x.BeginTransaction(capPublisherMock.Object, false))
            .Returns(transactionMock.Object);

        writeRepositoryMock
            .Setup(x => x.AddIndividualTask(It.IsAny<IndividualTask>()))
            .Returns(Task.CompletedTask);

        capPublisherMock
            .Setup(x => x.PublishDelayAsync(
                It.IsAny<TimeSpan>(),
                "IndividualTaskCreated",
                It.IsAny<IndividualTaskCreatedIntegrationEvent>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        transactionMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateIndividualTaskCommandHandler(
            writeRepositoryMock.Object,
            unitOfWorkMock.Object,
            capPublisherMock.Object);

        var request = new CreateIndividualTaskCommandRequest(
            Guid.NewGuid(),
            "Title",
            "Description",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            Guid.NewGuid(),
            1);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        writeRepositoryMock.Verify(x => x.AddIndividualTask(It.IsAny<IndividualTask>()), Times.Once);
        capPublisherMock.Verify(x => x.PublishDelayAsync(
            It.IsAny<TimeSpan>(),
            "IndividualTaskCreated",
            It.IsAny<IndividualTaskCreatedIntegrationEvent>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenException_ShouldRollback()
    {
        // Arrange
        var writeRepositoryMock = new Mock<IProjectManagementWriteRepository>();
        var unitOfWorkMock = new Mock<ICapUnitOfWork>();
        var capPublisherMock = new Mock<ICapPublisher>();
        var transactionMock = new Mock<IDbContextTransaction>();

        unitOfWorkMock
            .Setup(x => x.BeginTransaction(capPublisherMock.Object, false))
            .Returns(transactionMock.Object);

        writeRepositoryMock
            .Setup(x => x.AddIndividualTask(It.IsAny<IndividualTask>()))
            .ThrowsAsync(new InvalidOperationException("failed"));

        transactionMock
            .Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateIndividualTaskCommandHandler(
            writeRepositoryMock.Object,
            unitOfWorkMock.Object,
            capPublisherMock.Object);

        var request = new CreateIndividualTaskCommandRequest(
            Guid.NewGuid(),
            "Title",
            "Description",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            Guid.NewGuid(),
            1);

        // Act
        Func<Task> act = () => handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
