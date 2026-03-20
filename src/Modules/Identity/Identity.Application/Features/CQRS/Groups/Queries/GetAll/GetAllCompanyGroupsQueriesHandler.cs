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

            return companyGroups
                .Where(group => !string.IsNullOrWhiteSpace(group.Name))
                .GroupBy(group => group.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(grouped => new GetAllCompanyGroupsQueriesResponse
                {
                    GroupId = grouped
                        .Select(group => group.Id)
                        .FirstOrDefault(id => id != Guid.Empty),
                    GroupName = grouped.First().Name.Trim(),
                    WorkerUserIds = grouped
                        .SelectMany(group => group.Users)
                        .Select(member => member.UserId)
                        .Where(id => id != Guid.Empty)
                        .Distinct()
                        .ToList(),
                    WorkerName = grouped
                        .SelectMany(group => group.Users)
                        .Select(member => member.User?.Name?.Trim())
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .Cast<string>()
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList(),
                    DepartmenName = grouped
                        .SelectMany(group => group.Users)
                        .SelectMany(member => member.User?.DepartmentMembers ?? [])
                        .Select(dm => dm.Department?.Name?.Trim())
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .Cast<string>()
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .DefaultIfEmpty("Departman atamasi yapilmamis")
                        .ToList(),
                    LeaderUserIds = grouped
                        .SelectMany(group => group.Users)
                        .Where(member => member.GroupRolesId == 1)
                        .Select(member => member.UserId)
                        .Where(id => id != Guid.Empty)
                        .Distinct()
                        .ToList()
                })
                .OrderBy(dto => dto.GroupName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
