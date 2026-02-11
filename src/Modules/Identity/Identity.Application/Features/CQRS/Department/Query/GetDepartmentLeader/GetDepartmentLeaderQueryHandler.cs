using FlashMediator;
using Identity.Application.Repositories;
using Identity.Domain.Entities;

namespace Identity.Application.Features.CQRS.Department.Query.GetDepartmentLeader
{
    public class GetDepartmentLeaderQueryHandler : IRequestHandler<GetDepartmentLeaderQueryRequest, Guid?>
    {
        private readonly IReadRepository<DepartmentMember, int> _repository;

        public GetDepartmentLeaderQueryHandler(IReadRepository<DepartmentMember, int> repository)
        {
            _repository = repository;
        }

        public async Task Handle(GetDepartmentLeaderQueryRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetDepartmentLeaderIdAsync(request.DepartmentId);
        }
    }
}
