using FlashMediator;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TaskFlow.BuildingBlocks.Behaviors;
using TaskFlow.BuildingBlocks.Interfaces;

namespace BuildingBlocks.Tests.Behaviors;

public class RedisCacheBehaviorTests
{
    [Fact]
    public async Task Handle_WhenCacheHit_ShouldReturnCachedValue()
    {
        // Arrange
        var cacheServiceMock = new Mock<ICacheService>();

        var query = new CacheableTestQuery("cache:key", TimeSpan.FromMinutes(5));
        cacheServiceMock
            .Setup(x => x.GetAsync<string>(query.CacheKey))
            .ReturnsAsync("cached");

        var behavior = new RedisCacheBehavior<CacheableTestQuery, string>(
            cacheServiceMock.Object,
            NullLogger<RedisCacheBehavior<CacheableTestQuery, string>>.Instance);

        var nextCalled = false;
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("fresh");
        };

        // Act
        var response = await behavior.Handle(query, next, CancellationToken.None);

        // Assert
        Assert.Equal("cached", response);
        Assert.False(nextCalled);
        cacheServiceMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_ShouldCallNextAndCacheResult()
    {
        // Arrange
        var cacheServiceMock = new Mock<ICacheService>();

        var query = new CacheableTestQuery("cache:key", TimeSpan.FromMinutes(5));
        cacheServiceMock
            .Setup(x => x.GetAsync<string>(query.CacheKey))
            .Returns(Task.FromResult<string>(null!));

        var behavior = new RedisCacheBehavior<CacheableTestQuery, string>(
            cacheServiceMock.Object,
            NullLogger<RedisCacheBehavior<CacheableTestQuery, string>>.Instance);

        var nextCalled = false;
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("fresh");
        };

        // Act
        var response = await behavior.Handle(query, next, CancellationToken.None);

        // Assert
        Assert.Equal("fresh", response);
        Assert.True(nextCalled);
        cacheServiceMock.Verify(x => x.SetAsync(query.CacheKey, "fresh", query.ExpirationTime), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCacheThrows_ShouldFallbackToNext()
    {
        // Arrange
        var cacheServiceMock = new Mock<ICacheService>();

        var query = new CacheableTestQuery("cache:key", TimeSpan.FromMinutes(5));
        cacheServiceMock
            .Setup(x => x.GetAsync<string>(query.CacheKey))
            .ThrowsAsync(new Exception("cache down"));

        var behavior = new RedisCacheBehavior<CacheableTestQuery, string>(
            cacheServiceMock.Object,
            NullLogger<RedisCacheBehavior<CacheableTestQuery, string>>.Instance);

        var nextCalled = false;
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("fallback");
        };

        // Act
        var response = await behavior.Handle(query, next, CancellationToken.None);

        // Assert
        Assert.Equal("fallback", response);
        Assert.True(nextCalled);
        cacheServiceMock.Verify(x => x.SetAsync(query.CacheKey, "fallback", query.ExpirationTime), Times.Once);
    }

    public sealed record CacheableTestQuery(string CacheKey, TimeSpan? ExpirationTime) : ICacheableQuery;
}
