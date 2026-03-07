using System.Linq.Expressions;
using Identity.Application.Features.CQRS.Auth.Queries.SearchCompanyUsers;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace Identity.Tests.CQRS;

public class SearchCompanyUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldFilterByCompanyAndSearchText()
    {
        var companyId = Guid.NewGuid();
        var otherCompanyId = Guid.NewGuid();

        var users = new[]
        {
            CreateUser("Alice Johnson", "alice@taskflow.test", companyId),
            CreateUser("Bob Stone", "bob@taskflow.test", companyId),
            CreateUser("Alicia Other", "alicia@elsewhere.test", otherCompanyId)
        };

        var handler = new SearchCompanyUsersQueryHandler(CreateUserManager(users));
        var request = new SearchCompanyUsersQueryRequest
        {
            CompanyId = companyId,
            SearchText = "ali",
            Page = 1,
            PageSize = 10
        };

        var result = await handler.Handle(request, CancellationToken.None);

        var item = Assert.Single(result.Items);
        Assert.Equal(users[0].Id, item.Id);
        Assert.Equal("Alice Johnson", item.Name);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task Handle_WithInvalidPaging_ShouldApplyDefaults()
    {
        var companyId = Guid.NewGuid();
        var users = Enumerable.Range(1, 3)
            .Select(index => CreateUser($"User {index}", $"user{index}@taskflow.test", companyId))
            .ToArray();

        var handler = new SearchCompanyUsersQueryHandler(CreateUserManager(users));
        var request = new SearchCompanyUsersQueryRequest
        {
            CompanyId = companyId,
            SearchText = string.Empty,
            Page = 0,
            PageSize = 0
        };

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Equal(3, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
    }

    private static UserManager<User> CreateUserManager(IEnumerable<User> users)
    {
        var storeMock = new Mock<IQueryableUserStore<User>>();
        storeMock.Setup(store => store.Users).Returns(new TestAsyncEnumerable<User>(users));

        return new UserManager<User>(
            storeMock.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
    }

    private static User CreateUser(string name, string email, Guid companyId)
    {
        var user = User.Create(name, email, companyId);
        user.Id = Guid.NewGuid();
        return user;
    }

    private sealed class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
    {
        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression) => inner.Execute(expression)!;

        public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executeMethod = typeof(IQueryProvider)
                .GetMethods()
                .Single(method => method.Name == nameof(IQueryProvider.Execute) && method.IsGenericMethod && method.GetParameters().Length == 1);
            var executionResult = executeMethod
                .MakeGenericMethod(expectedResultType)
                .Invoke(inner, [expression]);

            return (TResult)typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, [executionResult])!;
        }
    }

    private sealed class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(((IEnumerable<T>)this).GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    private sealed class TestAsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
    {
        public T Current => inner.Current;

        public ValueTask DisposeAsync()
        {
            inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync() => new(inner.MoveNext());
    }
}