using FlashMediator;
using Identity.Application.Repositories;
using Identity.Domain.Entities;

namespace Identity.Application.Features.CQRS.Department.Query.GetDepartmentLeader
{
    public class GetDepartmentLeaderQueryHandler : IRequestHandler<GetDepartmentLeaderQueryRequest, GetDepartmentLeaderQueryResponse>
    {
        private readonly IReadRepository<DepartmentMember, int> _repository;

        public GetDepartmentLeaderQueryHandler(IReadRepository<DepartmentMember, int> repository)
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
