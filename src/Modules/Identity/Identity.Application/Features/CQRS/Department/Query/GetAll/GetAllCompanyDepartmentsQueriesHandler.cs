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
            var departmentsPage = await _readRepository.GetAllAsync(
                pageSize: 100,
                page: 1,
                trackChanges: false);

            return departmentsPage.Items
                .Where(item => item.CompanyId == request.CompanyId)
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
