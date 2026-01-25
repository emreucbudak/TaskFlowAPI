using FlashMediator;
using Identity.Application.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Contracts.UserGroups;

namespace Identity.Application.Features.CQRS.UserGroups.Queries
{
    public class GetUserAllGroupsNameQueriesHandler : IRequestHandler<GetUserAllGroupsNameQueriesRequest, List<string>>
    {
        private readonly IReadRepository<Domain.Entities.Groups,Guid> _readRepository;
        public GetUserAllGroupsNameQueriesHandler(IReadRepository<Domain.Entities.Groups, Guid> readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<List<string>> Handle(GetUserAllGroupsNameQueriesRequest request, CancellationToken cancellationToken)
        {
            var groups = await _readRepository.GetAllAsync(false);
            var userGroupsNames = groups.Where(g => g.Users.Any(ug => ug.UserId == request.userId))
                                        .Select(g => g.Name)
                                        .ToList();
            return userGroupsNames;
        }
    }
}
