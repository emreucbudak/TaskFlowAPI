using FlashMediator;

namespace Identity.Application.Features.CQRS.Company.Queries.GetAll
{
    public class GetAllCompaniesQueriesHandler : IRequestHandler<GetAllCompaniesQueriesRequest, List<GetAllCompaniesQueriesResponse>>
    {
        public Task<List<GetAllCompaniesQueriesResponse>> Handle(GetAllCompaniesQueriesRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
