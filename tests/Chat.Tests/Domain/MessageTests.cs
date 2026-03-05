using Chat.Domain.Entities;

namespace Chat.Tests.Domain;

public class MessageTests
{
    [Fact]
    public void Create_ShouldSetDefaultValues()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();

        // Act
        var message = Message.Create("Hello", senderId, receiverId);

        // Assert
        Assert.False(message.IsRead);
        Assert.False(message.IsDeleted);
        Assert.False(message.IsEdited);
        Assert.False(message.IsDelivered);
    }

    [Fact]
    public void Create_ForGroup_ShouldSetGroupIdAndNullReceiver()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        var groupId = Guid.NewGuid();

        // Act
        var message = Message.Create("Hello Group", senderId, null, groupId);

        // Assert
        Assert.Equal(groupId, message.GroupId);
        Assert.Null(message.ReceiverId);
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetFlag()
    {
        // Arrange
        var message = Message.Create("Hello", Guid.NewGuid(), Guid.NewGuid());

        // Act
        message.MarkAsDeleted();

        // Assert
        Assert.True(message.IsDeleted);
    }

    [Fact]
    public void MarkAsEdited_ShouldSetFlag()
    {
        // Arrange
        var message = Message.Create("Hello", Guid.NewGuid(), Guid.NewGuid());

        // Act
        message.MarkAsEdited();

        // Assert
        Assert.True(message.IsEdited);
    }

    [Fact]
    public void MarkAsDelivered_ShouldSetFlagAndTime()
    {
        // Arrange
        var message = Message.Create("Hello", Guid.NewGuid(), Guid.NewGuid());

        // Act
        message.MarkAsDelivered();

        // Assert
        Assert.True(message.IsDelivered);
        Assert.NotNull(message.DeliveredTime);
    }

    [Fact]
    public void UpdateContent_ShouldChangeContent()
    {
        // Arrange
        var message = Message.Create("Old", Guid.NewGuid(), Guid.NewGuid());

        // Act
        message.UpdateContent("New");

        // Assert
        Assert.Equal("New", message.Content);
    }

    [Fact]
    public void MarkAsRead_ShouldSetFlag()
    {
        // Arrange
        var message = Message.Create("Hello", Guid.NewGuid(), Guid.NewGuid());

        // Act
        message.MarkAsRead(true);

        // Assert
        Assert.True(message.IsRead);
    }
}
