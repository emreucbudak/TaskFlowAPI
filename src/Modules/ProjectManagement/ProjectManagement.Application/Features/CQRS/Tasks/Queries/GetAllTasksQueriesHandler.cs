
using FlashMediator;
using ProjectManagement.Application.Features.CQRS.Tasks.Queries.DTOS;
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

        public async Task<List<GetAllTasksQueriesResponse>> Handle(GetAllTasksQueriesRequest request, CancellationToken cancellationToken)
        {
            var tasks = await _repository.GetAllTasks(false);
            return tasks.Select(x=> new GetAllTasksQueriesResponse()
            {
                TaskName = x.TaskName,
                DeadlineTime = x.DeadlineTime,
                Description = x.Description,
                StatusName = x.GetTaskStatus(),
                CategoryName = x.GetTaskPriorityCategory(),
                TaskAnswers = x.taskAnswers.Select(x=> new TaskAnswerDTO()
                {
                    AnswerText = x.AnswerText,
                    SenderId = x.SenderId,
                }).ToList(),
                SubTasks = x.subtask.Select(x=> new SubTaskDTO()
                {
                    Description = x.Description,
                    AssignedUserId = x.AssignedUserId,
                    TaskTitle = x.TaskTitle,
                    StatusName = x.GetTaskStatus()
                }).ToList(),                
            }).ToList();
        }
    }
}
