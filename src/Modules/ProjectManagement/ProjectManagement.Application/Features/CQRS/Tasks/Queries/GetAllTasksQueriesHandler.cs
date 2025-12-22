using FlashMediator.src.FlashMediator.Contracts;
using ProjectManagement.Application.Repositories;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries
{
    public class GetAllTasksQueriesHandler : IRequestHandler<GetAllTasksQueriesRequest, List<GetAllTasksQueriesResponse>>
    {
        private readonly IProjectManagementReadRepository _repository;

        public GetAllTasksQueriesHandler(IProjectManagementReadRepository repository)
        {
            _repository = repository;
        }

        public Task<List<GetAllTasksQueriesResponse>> Handle(GetAllTasksQueriesRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
