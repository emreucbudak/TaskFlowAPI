using FlashMediator;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Create
{
    public record CreateTasksCommandRequest(
        string TaskName,
        string Description,
        DateTime DeadlineTime,
        int TaskPriorityCategoryId) : IRequest
    {
    }
}
