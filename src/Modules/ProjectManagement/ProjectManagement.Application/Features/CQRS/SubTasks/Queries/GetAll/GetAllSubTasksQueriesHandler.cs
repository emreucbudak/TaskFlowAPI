using FlashMediator;
using ProjectManagement.Application.Repositories;

namespace ProjectManagement.Application.Features.CQRS.SubTasks.Queries.GetAll
{
    public class GetAllSubTasksQueriesHandler : IRequestHandler<GetAllSubTasksQueriesRequest, List<GetAllSubTasksQueriesResponse>>
    {
        private readonly IProjectManagementReadRepository _repository;

        public GetAllSubTasksQueriesHandler(IProjectManagementReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetAllSubTasksQueriesResponse>> Handle(GetAllSubTasksQueriesRequest request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetTask(request.TaskId, false);
            ArgumentNullException.ThrowIfNull(task);
            var subTask = task.GetAllSubTasks();
            return subTask.Select(x => new GetAllSubTasksQueriesResponse
            {
                Description = x.Description,
                AssignedUserId = x.AssignedUserId,
                TaskStatusId = x.TaskStatusId,
                TaskTitle = x.TaskTitle,
            }).ToList();
        }
    }
}
