using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Complete;
using ProjectManagement.Application.Features.CQRS.IndividualTasks.Exceptions;
using ProjectManagement.Application.IntegrationEvents;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;
using IndividualTask = ProjectManagement.Domain.Entities.IndividualTasks;

namespace ProjectManagement.Tests.CQRS;

public class CompleteIndividualTaskCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenTaskNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var readRepositoryMock = new Mock<IProjectManagementReadRepository>();
        var writeRepositoryMock = new Mock<IProjectManagementWriteRepository>();
        var unitOfWorkMock = new Mock<ICapUnitOfWork>();
        var capPublisherMock = new Mock<ICapPublisher>();
        var transactionMock = new Mock<IDbContextTransaction>();

        unitOfWorkMock
            .Setup(x => x.BeginTransaction(capPublisherMock.Object, false))
            .Returns(transactionMock.Object);

        readRepositoryMock
            .Setup(x => x.GetIndividualTask(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IndividualTask)null!);

        var handler = new CompleteIndividualTaskCommandHandler(
            readRepositoryMock.Object,
            writeRepositoryMock.Object,
            unitOfWorkMock.Object,
            capPublisherMock.Object);

        var request = new CompleteIndividualTaskCommandRequest(Guid.NewGuid());

        // Act
        Func<Task> act = () => handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<IndividualTaskNotFoundException>(act);
        writeRepositoryMock.Verify(x => x.DeleteIndividualTask(It.IsAny<IndividualTask>()), Times.Never);
        capPublisherMock.Verify(x => x.PublishAsync(
            It.IsAny<string>(),
            It.IsAny<IndividualTaskCompletedIntegrationEvent>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTaskExists_ShouldDeleteAndPublishEvent()
    {
        // Arrange
        var readRepositoryMock = new Mock<IProjectManagementReadRepository>();
        var writeRepositoryMock = new Mock<IProjectManagementWriteRepository>();
        var unitOfWorkMock = new Mock<ICapUnitOfWork>();
        var capPublisherMock = new Mock<ICapPublisher>();
        var transactionMock = new Mock<IDbContextTransaction>();

        unitOfWorkMock
            .Setup(x => x.BeginTransaction(capPublisherMock.Object, false))
            .Returns(transactionMock.Object);

        var task = new IndividualTask(
            Guid.NewGuid(),
            "Task",
            "Description",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            1);

        readRepositoryMock
            .Setup(x => x.GetIndividualTask(task.Id, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        writeRepositoryMock
            .Setup(x => x.DeleteIndividualTask(task))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        capPublisherMock
            .Setup(x => x.PublishAsync(
                "IndividualTaskCompleted",
                It.IsAny<IndividualTaskCompletedIntegrationEvent>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        transactionMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CompleteIndividualTaskCommandHandler(
            readRepositoryMock.Object,
            writeRepositoryMock.Object,
            unitOfWorkMock.Object,
            capPublisherMock.Object);

        var request = new CompleteIndividualTaskCommandRequest(task.Id);

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        writeRepositoryMock.Verify(x => x.DeleteIndividualTask(task), Times.Once);
        capPublisherMock.Verify(x => x.PublishAsync(
            "IndividualTaskCompleted",
            It.IsAny<IndividualTaskCompletedIntegrationEvent>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
