using FlashMediator;
using Identity.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Features.CQRS.Groups.Queries.GetAll
{
    public class GetAllCompanyGroupsQueriesHandler : IRequestHandler<GetAllCompanyGroupsQueriesRequest, List<GetAllCompanyGroupsQueriesResponse>>
    {
        private readonly IReadRepository<Domain.Entities.Groups,Guid> _readRepository;

        public GetAllCompanyGroupsQueriesHandler(IReadRepository<Domain.Entities.Groups, Guid> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<GetAllCompanyGroupsQueriesResponse>> Handle(GetAllCompanyGroupsQueriesRequest request, CancellationToken cancellationToken)
        {
            var company = await _readRepository.GetByIdAsync(false,request.CompanyId, x => x.Include(y => y.Users).ThenInclude(x => x.User).ThenInclude(x => x.DepartmentMembers).ThenInclude(dm => dm.Department));
            return company.Users.Select(group => new GetAllCompanyGroupsQueriesResponse
            {
                GroupName = company.Name,
                WorkerName = company.Users.Select(user => user.User.Name).ToList(),
                DepartmenName = company.Users.Select(user => user.User.DepartmentMembers.Any() ? string.Join(", ", user.User.DepartmentMembers.Select(dm => dm.Department.Name)) : "Departman ataması yapılmamış").ToList()
            }).ToList();
        }
    }
}
