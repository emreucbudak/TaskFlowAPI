
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
            List<Domain.Entities.Task> tasks;
            try
            {
                tasks = await _repository.GetAllTasks(false, request.PageNumber, request.PageSize);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Task list could not be loaded. Returning empty page. CompanyId={CompanyId}, PageNumber={PageNumber}, PageSize={PageSize}",
                    request.CompanyId,
                    request.PageNumber,
                    request.PageSize);

                return new PagedResult<GetAllTasksQueriesResponse>
                {
                    Items = [],
                    TotalCount = 0,
                    Page = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

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
                TotalCount = tasks.Count(),
                Page = request.PageNumber,
                PageSize = request.PageSize,
                
            };

        }
    }
}
