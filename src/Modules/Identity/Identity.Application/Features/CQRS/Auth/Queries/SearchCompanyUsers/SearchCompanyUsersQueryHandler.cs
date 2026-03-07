using FlashMediator;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Common;
using UserResponse = Identity.Application.Features.CQRS.Auth.Queries.GetAllCompanyUsers.GetAllCompanyUsersQueriesResponse;

namespace Identity.Application.Features.CQRS.Auth.Queries.SearchCompanyUsers;

public sealed class SearchCompanyUsersQueryHandler(UserManager<User> userManager)
    : IRequestHandler<SearchCompanyUsersQueryRequest, PagedResult<UserResponse>>
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;

    public async Task<PagedResult<UserResponse>> Handle(SearchCompanyUsersQueryRequest request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? DefaultPageSize : Math.Min(request.PageSize, MaxPageSize);
        var searchText = request.SearchText?.Trim();

        var query = userManager.Users
            .AsNoTracking()
            .Where(user => user.CompanyId == request.CompanyId);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var normalizedSearchText = searchText.ToLowerInvariant();
            query = query.Where(user =>
                ((user.Name ?? string.Empty).ToLower().Contains(normalizedSearchText)) ||
                ((user.Email ?? string.Empty).ToLower().Contains(normalizedSearchText)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(user => user.Name ?? user.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name ?? user.Email ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<UserResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}