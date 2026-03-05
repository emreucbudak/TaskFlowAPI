using Notification.Domain.Models;

namespace Notification.Tests.Domain;

public class NotificationMessageTests
{
    [Fact]
    public void Constructor_WithValidParams_ShouldCreate()
    {
        // Arrange
        var title = "Title";
        var description = "Description";
        var sendTime = DateTime.UtcNow;
        var isRead = false;
        var receiverUserId = Guid.NewGuid();

        // Act
        var message = new NotificationMessage(title, description, sendTime, isRead, receiverUserId);

        // Assert
        Assert.Equal(title, message.Title);
        Assert.Equal(description, message.Description);
        Assert.Equal(sendTime, message.SendTime);
        Assert.False(message.IsRead);
        Assert.Equal(receiverUserId, message.ReceiverUserId);
    }

    [Fact]
    public void Constructor_WithEmptyReceiverId_ShouldThrow()
    {
        // Arrange
        var receiverUserId = Guid.Empty;

        // Act
        Action act = () => new NotificationMessage("Title", "Description", DateTime.UtcNow, false, receiverUserId);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_WithEmptyDescription_ShouldThrow()
    {
        // Arrange
        var receiverUserId = Guid.NewGuid();

        // Act
        Action act = () => new NotificationMessage("Title", string.Empty, DateTime.UtcNow, false, receiverUserId);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void MarkAsRead_ShouldSetIsReadToTrue()
    {
        // Arrange
        var message = new NotificationMessage("Title", "Description", DateTime.UtcNow, false, Guid.NewGuid());

        // Act
        message.MarkAsRead();

        // Assert
        Assert.True(message.IsRead);
    }

    [Fact]
    public void MarkAsRead_CalledMultipleTimes_ShouldStayTrue()
    {
        // Arrange
        var message = new NotificationMessage("Title", "Description", DateTime.UtcNow, false, Guid.NewGuid());

        // Act
        message.MarkAsRead();
        message.MarkAsRead();

        // Assert
        Assert.True(message.IsRead);
    }
}
