using Chat.Application.ChatNotification;
using Chat.Application.Exceptions;
using Chat.Application.Features.CQRS.Message.Command.Create;
using Chat.Application.Repositories;
using Chat.Application.Services;
using Chat.Application.UnitOfWork;
using Identity.Application.Services;
using Moq;

namespace Chat.Tests.Services;

public class MessageControlServiceTests
{
    [Fact]
    public async Task HandleMessageCreation_WhenUserNotAuthenticated_ShouldThrowMessageAuthException()
    {
        // Arrange
        var service = CreateService(out _, out _, out _, out var currentUserServiceMock, out _);
        currentUserServiceMock.SetupGet(x => x.UserId).Returns((Guid?)null);

        var request = new CreateMessageCommandRequest("Hello", Guid.NewGuid(), Guid.NewGuid(), null);

        // Act
        Func<Task> act = () => service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MessageAuthException>(act);
    }

    [Fact]
    public async Task HandleMessageCreation_WhenSenderMismatch_ShouldThrowMessageAuthException()
    {
        // Arrange
        var service = CreateService(out _, out _, out _, out var currentUserServiceMock, out _);
        currentUserServiceMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var request = new CreateMessageCommandRequest("Hello", Guid.NewGuid(), Guid.NewGuid(), null);

        // Act
        Func<Task> act = () => service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MessageAuthException>(act);
    }

    [Fact]
    public async Task HandleMessageCreation_WithEmptyContent_ShouldThrowMessageControlException()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var service = CreateService(out _, out _, out _, out var currentUserServiceMock, out _);
        currentUserServiceMock.SetupGet(x => x.UserId).Returns(senderId);

        var request = new CreateMessageCommandRequest("   ", senderId, Guid.NewGuid(), null);

        // Act
        Func<Task> act = () => service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MessageControlException>(act);
    }

    [Fact]
    public async Task HandleMessageCreation_WithContentOver1000Chars_ShouldThrowMessageControlException()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var service = CreateService(out _, out _, out _, out var currentUserServiceMock, out _);
        currentUserServiceMock.SetupGet(x => x.UserId).Returns(senderId);

        var content = new string('a', 1001);
        var request = new CreateMessageCommandRequest(content, senderId, Guid.NewGuid(), null);

        // Act
        Func<Task> act = () => service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MessageControlException>(act);
    }

    [Fact]
    public async Task HandleMessageCreation_WithXSSContent_ShouldThrowMessageControlException()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var service = CreateService(out _, out _, out _, out var currentUserServiceMock, out _);
        currentUserServiceMock.SetupGet(x => x.UserId).Returns(senderId);

        var request = new CreateMessageCommandRequest("<script>alert(1)</script>", senderId, Guid.NewGuid(), null);

        // Act
        Func<Task> act = () => service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MessageControlException>(act);
    }

    [Fact]
    public async Task HandleMessageCreation_WithBothReceiverAndGroup_ShouldThrow()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var service = CreateService(out _, out _, out _, out var currentUserServiceMock, out _);
        currentUserServiceMock.SetupGet(x => x.UserId).Returns(senderId);

        var request = new CreateMessageCommandRequest("Hello", senderId, Guid.NewGuid(), Guid.NewGuid());

        // Act
        Func<Task> act = () => service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MessageControlException>(act);
    }

    [Fact]
    public async Task HandleMessageCreation_WithNeitherReceiverNorGroup_ShouldThrow()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var service = CreateService(out _, out _, out _, out var currentUserServiceMock, out _);
        currentUserServiceMock.SetupGet(x => x.UserId).Returns(senderId);

        var request = new CreateMessageCommandRequest("Hello", senderId, null, null);

        // Act
        Func<Task> act = () => service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MessageControlException>(act);
    }

    [Fact]
    public async Task HandleMessageCreation_ForGroupWhenNotMember_ShouldThrow()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var groupId = Guid.NewGuid();

        var service = CreateService(
            out _,
            out _,
            out _,
            out var currentUserServiceMock,
            out var groupValidationServiceMock);

        currentUserServiceMock.SetupGet(x => x.UserId).Returns(senderId);
        groupValidationServiceMock
            .Setup(x => x.ValidateGroupMembershipAsync(senderId, groupId))
            .ReturnsAsync(false);

        var request = new CreateMessageCommandRequest("Hello", senderId, null, groupId);

        // Act
        Func<Task> act = () => service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MessageControlException>(act);
    }

    [Fact]
    public async Task HandleMessageCreation_ValidDirectMessage_ShouldSaveAndNotify()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();

        var service = CreateService(
            out var messageWriteRepositoryMock,
            out var unitOfWorkMock,
            out var chatNotificationServiceMock,
            out var currentUserServiceMock,
            out var groupValidationServiceMock);

        currentUserServiceMock.SetupGet(x => x.UserId).Returns(senderId);
        messageWriteRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Chat.Domain.Entities.Message>()))
            .ReturnsAsync((Chat.Domain.Entities.Message m) => m);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var request = new CreateMessageCommandRequest("Hello", senderId, receiverId, null);

        // Act
        await service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        messageWriteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Chat.Domain.Entities.Message>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        chatNotificationServiceMock.Verify(x => x.SendMessageToUserAsync(receiverId, It.IsAny<object>()), Times.Once);
        chatNotificationServiceMock.Verify(x => x.SendMessageToGroupAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        groupValidationServiceMock.Verify(x => x.ValidateGroupMembershipAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task HandleMessageCreation_ValidGroupMessage_ShouldSaveAndNotifyGroup()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var groupId = Guid.NewGuid();

        var service = CreateService(
            out var messageWriteRepositoryMock,
            out var unitOfWorkMock,
            out var chatNotificationServiceMock,
            out var currentUserServiceMock,
            out var groupValidationServiceMock);

        currentUserServiceMock.SetupGet(x => x.UserId).Returns(senderId);
        groupValidationServiceMock
            .Setup(x => x.ValidateGroupMembershipAsync(senderId, groupId))
            .ReturnsAsync(true);
        messageWriteRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Chat.Domain.Entities.Message>()))
            .ReturnsAsync((Chat.Domain.Entities.Message m) => m);
        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var request = new CreateMessageCommandRequest("Hello Group", senderId, null, groupId);

        // Act
        await service.HandleMessageCreationAsync(request, CancellationToken.None);

        // Assert
        messageWriteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Chat.Domain.Entities.Message>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        chatNotificationServiceMock.Verify(x => x.SendMessageToGroupAsync(groupId, It.IsAny<object>()), Times.Once);
        chatNotificationServiceMock.Verify(x => x.SendMessageToUserAsync(It.IsAny<Guid>(), It.IsAny<object>()), Times.Never);
        groupValidationServiceMock.Verify(x => x.ValidateGroupMembershipAsync(senderId, groupId), Times.Once);
    }

    private static MessageControlService CreateService(
        out Mock<IMessageWriteRepository> messageWriteRepositoryMock,
        out Mock<IChatUnitOfWork> unitOfWorkMock,
        out Mock<IChatNotificationService> chatNotificationServiceMock,
        out Mock<ICurrentUserService> currentUserServiceMock,
        out Mock<IGroupValidationService> groupValidationServiceMock)
    {
        messageWriteRepositoryMock = new Mock<IMessageWriteRepository>();
        unitOfWorkMock = new Mock<IChatUnitOfWork>();
        chatNotificationServiceMock = new Mock<IChatNotificationService>();
        currentUserServiceMock = new Mock<ICurrentUserService>();
        groupValidationServiceMock = new Mock<IGroupValidationService>();

        return new MessageControlService(
            messageWriteRepositoryMock.Object,
            unitOfWorkMock.Object,
            chatNotificationServiceMock.Object,
            currentUserServiceMock.Object,
            groupValidationServiceMock.Object);
    }
}
