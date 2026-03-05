using FlashMediator;
using Identity.Application.Repositories;

namespace Identity.Application.Features.CQRS.Department.Query.GetDepartmentLeader
{
    public class GetDepartmentLeaderQueryHandler : IRequestHandler<GetDepartmentLeaderQueryRequest, GetDepartmentLeaderQueryResponse>
    {
        private readonly IDepartmentReadRepository _repository;

        public GetDepartmentLeaderQueryHandler(IDepartmentReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetDepartmentLeaderQueryResponse> Handle(GetDepartmentLeaderQueryRequest request, CancellationToken cancellationToken)
        {
            return new GetDepartmentLeaderQueryResponse
            {
                LeaderId = await _repository.GetDepartmentLeaderIdAsync(request.DepartmentId)
            };
        }
    }
}
