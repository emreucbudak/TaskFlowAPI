using FlashMediator;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Features.CQRS.Tasks.Queries.DTOS;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries
{
    public class GetAllTasksQueriesHandler : IRequestHandler<GetAllTasksQueriesRequest, PagedResult<GetAllTasksQueriesResponse>>
    {
        private readonly IProjectManagementReadRepository _repository;
        private readonly ILogger<GetAllTasksQueriesHandler> _logger;

        public GetAllTasksQueriesHandler(
            IProjectManagementReadRepository repository,
            ILogger<GetAllTasksQueriesHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedResult<GetAllTasksQueriesResponse>> Handle(GetAllTasksQueriesRequest request, CancellationToken cancellationToken)
        {
            var (tasks, totalCount) = await _repository.GetAllTasks(false, request.PageNumber, request.PageSize, cancellationToken);

            return new PagedResult<GetAllTasksQueriesResponse>
            {
                Items = tasks.Select(t => new GetAllTasksQueriesResponse
                {
                    TaskName = t.TaskName,
                    Description = t.Description,
                    DeadlineTime = t.DeadlineTime,
                    StatusName = t.GetTaskStatus(),
                    CategoryName = "Grup",
                    TaskPriorityName = string.IsNullOrWhiteSpace(t.GetTaskPriorityCategory())
                        ? "Belirtilmedi"
                        : t.GetTaskPriorityCategory(),
                    SubTasks = t.GetAllSubTasks().Select(st => new SubTaskDTO
                    {
                        TaskTitle = st.TaskTitle,
                        Description = st.Description,
                        AssignedUserId = st.AssignedUserId
                    }).ToList()
                }).ToList(),
                TotalCount = totalCount,
                Page = request.PageNumber,
                PageSize = request.PageSize,
            };

        }
    }
}


