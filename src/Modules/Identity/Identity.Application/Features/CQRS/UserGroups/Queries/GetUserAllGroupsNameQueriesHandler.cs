using FlashMediator;
using Identity.Application.IDbContext;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Contracts.UserGroups;

namespace Identity.Application.Features.CQRS.UserGroups.Queries
{
    public class GetUserAllGroupsNameQueriesHandler : IRequestHandler<GetUserAllGroupsNameQueriesRequest, List<string>>
    {
        private readonly IIdentityManagementDbContext _context;

        public GetUserAllGroupsNameQueriesHandler(IIdentityManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> Handle(GetUserAllGroupsNameQueriesRequest request, CancellationToken cancellationToken)
        {
            return await _context.Groups
                .AsNoTracking()
                .Where(x=> x.Users.Any(x=> x.UserId == request.userId))
                .Select(g => g.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
