using FlashMediator;
using Identity.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Features.CQRS.Groups.Queries.GetAll
{
    public class GetAllCompanyGroupsQueriesHandler : IRequestHandler<GetAllCompanyGroupsQueriesRequest, List<GetAllCompanyGroupsQueriesResponse>>
    {
        private readonly IReadRepository<Domain.Entities.Groups, Guid> _readRepository;

        public GetAllCompanyGroupsQueriesHandler(IReadRepository<Domain.Entities.Groups, Guid> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<GetAllCompanyGroupsQueriesResponse>> Handle(GetAllCompanyGroupsQueriesRequest request, CancellationToken cancellationToken)
        {
            var groupsPage = await _readRepository.GetAllAsync(
                pageSize: 100,
                page: 1,
                trackChanges: false,
                inc: query => query
                    .Include(group => group.Users)
                    .ThenInclude(member => member.User)
                    .ThenInclude(user => user.DepartmentMembers)
                    .ThenInclude(departmentMember => departmentMember.Department));

            var companyGroups = groupsPage.Items
                .Where(group => group.CompanyId == request.CompanyId)
                .ToList();

            return companyGroups.Select(group => new GetAllCompanyGroupsQueriesResponse
            {
                GroupName = group.Name,
                WorkerName = group.Users
                    .Select(member => member.User?.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Cast<string>()
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList(),
                DepartmenName = group.Users
                    .SelectMany(member => member.User?.DepartmentMembers ?? [])
                    .Select(departmentMember => departmentMember.Department?.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Cast<string>()
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .DefaultIfEmpty("Departman atamasi yapilmamis")
                    .ToList()
            }).ToList();
        }
    }
}
