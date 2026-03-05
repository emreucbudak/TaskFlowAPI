using Moq;
using Notification.Application.Features.CQRS.Notification.Command.Create;
using Notification.Application.Repositories;
using TaskFlow.BuildingBlocks.UnitOfWork;

namespace Notification.Tests.CQRS;

public class CreateNotificationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidRequest_ShouldCallWriteRepoAndSave()
    {
        // Arrange
        var writeRepositoryMock = new Mock<INotificationWriteRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        writeRepositoryMock
            .Setup(x => x.SendNotification(It.IsAny<Notification.Domain.Models.NotificationMessage>()))
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateNotificationCommandHandler(
            writeRepositoryMock.Object,
            unitOfWorkMock.Object);

        var request = new CreateNotificationCommandRequest(
            "Title",
            "Description",
            DateTime.UtcNow,
            false,
            Guid.NewGuid());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        writeRepositoryMock.Verify(
            x => x.SendNotification(It.Is<Notification.Domain.Models.NotificationMessage>(n =>
                n.Title == request.Title &&
                n.Description == request.Description &&
                n.ReceiverUserId == request.ReceiverUserId)),
            Times.Once);

        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
