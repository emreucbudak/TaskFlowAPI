using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.CreateGroupTask;

public record CreateGroupTaskWithSubTasksCommandRequest(
    string TaskName,
    string Description,
    DateTime DeadlineTime,
    int TaskPriorityCategoryId,
    List<SubTaskAssignmentDto> SubTaskAssignments) : IRequest<Guid>;

public record SubTaskAssignmentDto(
    Guid AssignedUserId,
    string TaskTitle,
    string Description);
