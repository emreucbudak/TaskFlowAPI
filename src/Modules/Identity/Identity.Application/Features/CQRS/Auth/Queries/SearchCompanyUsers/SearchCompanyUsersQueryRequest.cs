using FlashMediator;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Auth.Queries.SearchCompanyUsers;

public sealed class SearchCompanyUsersQueryRequest : IRequest<PagedResult<GetAllCompanyUsers.GetAllCompanyUsersQueriesResponse>>, ICacheableQuery
{
    public Guid CompanyId { get; init; }
    public string SearchText { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public string CacheKey => $"searchcompanyusers:{CompanyId}:{SearchText?.Trim().ToLowerInvariant()}:{Page}:{PageSize}";

    public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(5);
}
