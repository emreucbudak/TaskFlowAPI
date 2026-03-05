using FlashMediator;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TaskFlow.BuildingBlocks.Behaviors;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Interfaces;

namespace BuildingBlocks.Tests.Behaviors;

public class LimitBehaviorTests
{
    [Fact]
    public async Task Handle_WhenCheckerNotFound_ShouldThrowNotSupportedException()
    {
        // Arrange
        var checkers = Enumerable.Empty<ISubscriptionLimitCheckerService>();

        var behavior = new LimitBehavior<LimitedTestRequest, string>(
            checkers,
            NullLogger<LimitBehavior<LimitedTestRequest, string>>.Instance);

        var request = new LimitedTestRequest(Guid.NewGuid(), LimitType.IndividualTask);
        RequestHandlerDelegate<string> next = () => Task.FromResult("ok");

        // Act
        Func<Task> act = () => behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotSupportedException>(act);
    }

    [Fact]
    public async Task Handle_WhenCheckerFound_ShouldCallCheckAndProceed()
    {
        // Arrange
        var checkerMock = new Mock<ISubscriptionLimitCheckerService>();
        checkerMock.SetupGet(x => x.LimitType).Returns(LimitType.IndividualTask);
        checkerMock.Setup(x => x.CheckLimitAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        var behavior = new LimitBehavior<LimitedTestRequest, string>(
            new[] { checkerMock.Object },
            NullLogger<LimitBehavior<LimitedTestRequest, string>>.Instance);

        var request = new LimitedTestRequest(Guid.NewGuid(), LimitType.IndividualTask);
        var nextCalled = false;
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("ok");
        };

        // Act
        var response = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("ok", response);
        checkerMock.Verify(x => x.CheckLimitAsync(request.TenantId), Times.Once);
        checkerMock.Verify(x => x.ReleaseLimitAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNextThrows_ShouldReleaseLimitAndRethrow()
    {
        // Arrange
        var checkerMock = new Mock<ISubscriptionLimitCheckerService>();
        checkerMock.SetupGet(x => x.LimitType).Returns(LimitType.IndividualTask);
        checkerMock.Setup(x => x.CheckLimitAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
        checkerMock.Setup(x => x.ReleaseLimitAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        var behavior = new LimitBehavior<LimitedTestRequest, string>(
            new[] { checkerMock.Object },
            NullLogger<LimitBehavior<LimitedTestRequest, string>>.Instance);

        var request = new LimitedTestRequest(Guid.NewGuid(), LimitType.IndividualTask);
        RequestHandlerDelegate<string> next = () => throw new InvalidOperationException("boom");

        // Act
        Func<Task> act = () => behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        checkerMock.Verify(x => x.CheckLimitAsync(request.TenantId), Times.Once);
        checkerMock.Verify(x => x.ReleaseLimitAsync(request.TenantId), Times.Once);
    }

    public sealed record LimitedTestRequest(Guid TenantId, LimitType limitType) : ILimitedQueryable;
}
