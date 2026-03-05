using FlashMediator;
using Identity.Application.Repositories;

namespace Identity.Application.Features.CQRS.Department.Query.GetAll
{
    public sealed class GetAllCompanyDepartmentsQueriesHandler
        : IRequestHandler<GetAllCompanyDepartmentsQueriesRequest, List<GetAllCompanyDepartmentsQueriesResponse>>
    {
        private readonly IReadRepository<Domain.Entities.Department, Guid> _readRepository;

        public GetAllCompanyDepartmentsQueriesHandler(IReadRepository<Domain.Entities.Department, Guid> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<List<GetAllCompanyDepartmentsQueriesResponse>> Handle(GetAllCompanyDepartmentsQueriesRequest request, CancellationToken cancellationToken)
        {
            const int pageSize = 100;
            var page = 1;
            var departments = new List<Domain.Entities.Department>();

            while (true)
            {
                var departmentsPage = await _readRepository.GetAllAsync(
                    pageSize: pageSize,
                    page: page,
                    trackChanges: false,
                    predicate: item => item.CompanyId == request.CompanyId);

                departments.AddRange(departmentsPage.Items);

                if (page * pageSize >= departmentsPage.TotalCount)
                {
                    break;
                }

                page++;
            }

            return departments
                .OrderBy(item => item.Name)
                .Select(item => new GetAllCompanyDepartmentsQueriesResponse
                {
                    Id = item.Id,
                    Name = item.Name
                })
                .ToList();
        }
    }
}
