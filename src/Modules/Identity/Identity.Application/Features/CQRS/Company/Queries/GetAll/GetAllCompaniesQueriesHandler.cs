using FlashMediator;
using Identity.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Application.Features.CQRS.Company.Queries.GetAll
{
    public class GetAllCompaniesQueriesHandler : IRequestHandler<GetAllCompaniesQueriesRequest, PagedResult<GetAllCompaniesQueriesResponse>>
    {
        private readonly IReadRepository<Domain.Entities.Company, Guid> _readRepository;

        public GetAllCompaniesQueriesHandler(IReadRepository<Domain.Entities.Company, Guid> readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<PagedResult<GetAllCompaniesQueriesResponse>> Handle(GetAllCompaniesQueriesRequest request, CancellationToken cancellationToken)
        {
            var companies = await  _readRepository.GetAllAsync(request.PageNumber, request.PageSize,trackChanges:false);
            return new PagedResult<GetAllCompaniesQueriesResponse>
            {
                Items = companies.Items.Select(c => new GetAllCompaniesQueriesResponse
                {
                    CompanyName = c.CompanyName,
                }).ToList(),
                TotalCount = companies.Items.Count(),
                Page = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
