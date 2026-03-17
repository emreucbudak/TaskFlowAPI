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
            const int pageSize = 100;
            var page = 1;
            var companyGroups = new List<Domain.Entities.Groups>();

            while (true)
            {
                var groupsPage = await _readRepository.GetAllAsync(
                    pageSize: pageSize,
                    page: page,
                    trackChanges: false,
                    inc: query => query
                        .Include(group => group.Users)
                        .ThenInclude(member => member.User)
                        .ThenInclude(user => user.DepartmentMembers)
                        .ThenInclude(departmentMember => departmentMember.Department),
                    predicate: group => group.CompanyId == request.CompanyId);

                companyGroups.AddRange(groupsPage.Items);

                if (page * pageSize >= groupsPage.TotalCount)
                {
                    break;
                }

                page++;
            }

            return companyGroups.Select(group => new GetAllCompanyGroupsQueriesResponse
            {
                GroupId = group.Id,
                GroupName = group.Name,
                WorkerUserIds = group.Users
                    .Select(member => member.UserId)
                    .Distinct()
                    .ToList(),
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
                    .ToList(),
                LeaderUserIds = group.Users
                    .Where(member => member.GroupRolesId == 1)
                    .Select(member => member.UserId)
                    .Distinct()
                    .ToList()
            }).ToList();
        }
    }
}
