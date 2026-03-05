using TaskFlow.BuildingBlocks.Common;

namespace BuildingBlocks.Tests.Common;

public class PagedResultTests
{
    [Fact]
    public void TotalPages_ShouldCalculateCorrectly()
    {
        // Arrange
        var result = new PagedResult<int>
        {
            Items = new List<int> { 1, 2, 3 },
            TotalCount = 25,
            Page = 1,
            PageSize = 10
        };

        // Act
        var totalPages = result.TotalPages;

        // Assert
        Assert.Equal(3, totalPages);
    }

    [Fact]
    public void TotalPages_WhenZeroCount_ShouldBeZero()
    {
        // Arrange
        var result = new PagedResult<int>
        {
            Items = Array.Empty<int>(),
            TotalCount = 0,
            Page = 1,
            PageSize = 10
        };

        // Act
        var totalPages = result.TotalPages;

        // Assert
        Assert.Equal(0, totalPages);
    }

    [Fact]
    public void HasPreviousPage_WhenFirstPage_ShouldBeFalse()
    {
        // Arrange
        var result = new PagedResult<int>
        {
            Items = new List<int> { 1 },
            TotalCount = 10,
            Page = 1,
            PageSize = 10
        };

        // Act
        var hasPreviousPage = result.HasPreviousPage;

        // Assert
        Assert.False(hasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_WhenSecondPage_ShouldBeTrue()
    {
        // Arrange
        var result = new PagedResult<int>
        {
            Items = new List<int> { 1 },
            TotalCount = 20,
            Page = 2,
            PageSize = 10
        };

        // Act
        var hasPreviousPage = result.HasPreviousPage;

        // Assert
        Assert.True(hasPreviousPage);
    }

    [Fact]
    public void HasNextPage_WhenLastPage_ShouldBeFalse()
    {
        // Arrange
        var result = new PagedResult<int>
        {
            Items = new List<int> { 1 },
            TotalCount = 20,
            Page = 2,
            PageSize = 10
        };

        // Act
        var hasNextPage = result.HasNextPage;

        // Assert
        Assert.False(hasNextPage);
    }

    [Fact]
    public void HasNextPage_WhenNotLastPage_ShouldBeTrue()
    {
        // Arrange
        var result = new PagedResult<int>
        {
            Items = new List<int> { 1 },
            TotalCount = 20,
            Page = 1,
            PageSize = 10
        };

        // Act
        var hasNextPage = result.HasNextPage;

        // Assert
        Assert.True(hasNextPage);
    }
}
