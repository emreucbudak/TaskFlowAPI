using FlashMediator;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Features.CQRS.Auth.Queries.GetAllCompanyUsers;

public sealed class GetAllCompanyUsersQueriesHandler
    : IRequestHandler<GetAllCompanyUsersQueriesRequest, List<GetAllCompanyUsersQueriesResponse>>
{
    private readonly UserManager<User> _userManager;

    public GetAllCompanyUsersQueriesHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<GetAllCompanyUsersQueriesResponse>> Handle(GetAllCompanyUsersQueriesRequest request, CancellationToken cancellationToken)
    {
        var usersQuery = _userManager.Users
            .AsQueryable()
            .Where(user => user.CompanyId == request.CompanyId);

        return await usersQuery
            .Select(user => new GetAllCompanyUsersQueriesResponse
            {
                Id = user.Id,
                Name = user.Name ?? user.Email ?? string.Empty
            })
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);
    }
}
