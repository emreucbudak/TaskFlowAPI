using FlashMediator;

namespace Identity.Application.Features.CQRS.Groups.Queries.GetAll
{
    public class GetAllCompanyGroupsQueriesRequest : IRequest<List<GetAllCompanyGroupsQueriesResponse>>
    {
        public Guid CompanyId { get; init; }
    }
}
