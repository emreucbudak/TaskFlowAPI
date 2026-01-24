using FlashMediator;
using Identity.Application.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.BuildingBlocks.Contracts.UserGroups;

namespace Identity.Application.Features.CQRS.UserGroups.Queries
{
    public class GetUserAllGroupsNameQueriesHandler : IRequestHandler<GetUserAllGroupsNameQueriesRequest, List<string>>
    {
        private readonly IReadRepository<Domain.Entities.Groups> _readRepository;
        public GetUserAllGroupsNameQueriesHandler(IReadRepository<Domain.Entities.Groups> readRepository)
        {
            _readRepository = readRepository;
        }
        public async Task<List<string>> Handle(GetUserAllGroupsNameQueriesRequest request, CancellationToken cancellationToken)
        {
            return await _readRepository
        }
    }
}
