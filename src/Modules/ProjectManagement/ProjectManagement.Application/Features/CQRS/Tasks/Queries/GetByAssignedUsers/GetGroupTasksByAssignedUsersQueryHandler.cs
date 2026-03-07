using FlashMediator;
using ProjectManagement.Application.Features.CQRS.Tasks.Queries.DTOS;
using ProjectManagement.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Queries.GetByAssignedUsers
{
    public sealed class GetGroupTasksByAssignedUsersQueryHandler(
        IProjectManagementReadRepository repository) : IRequestHandler<GetGroupTasksByAssignedUsersQueryRequest, PagedResult<GetAllTasksQueriesResponse>>
    {
        public async Task<PagedResult<GetAllTasksQueriesResponse>> Handle(GetGroupTasksByAssignedUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var assignedUserIds = request.AssignedUserIds?
                .Where(userId => userId != Guid.Empty)
                .Distinct()
                .ToArray() ?? [];

            if (assignedUserIds.Length == 0)
            {
                return new PagedResult<GetAllTasksQueriesResponse>
                {
                    Items = [],
                    TotalCount = 0,
                    Page = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            var (tasks, totalCount) = await repository.GetTasksByAssignedUserIds(
                false,
                request.PageNumber,
                request.PageSize,
                assignedUserIds,
                cancellationToken);

            return new PagedResult<GetAllTasksQueriesResponse>
            {
                Items = tasks.Select(MapTask).ToList(),
                TotalCount = totalCount,
                Page = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private static GetAllTasksQueriesResponse MapTask(ProjectManagement.Domain.Entities.Task task)
        {
            return new GetAllTasksQueriesResponse
            {
                TaskName = task.TaskName,
                Description = task.Description,
                DeadlineTime = task.DeadlineTime,
                StatusName = task.GetTaskStatus(),
                CategoryName = "Grup",
                TaskPriorityName = string.IsNullOrWhiteSpace(task.GetTaskPriorityCategory())
                    ? "Belirtilmedi"
                    : task.GetTaskPriorityCategory(),
                SubTasks = task.GetAllSubTasks().Select(subTask => new SubTaskDTO
                {
                    TaskTitle = subTask.TaskTitle,
                    Description = subTask.Description,
                    AssignedUserId = subTask.AssignedUserId,
                    StatusName = subTask.TaskStatus?.StatusName ?? string.Empty
                }).ToList()
            };
        }
    }
}