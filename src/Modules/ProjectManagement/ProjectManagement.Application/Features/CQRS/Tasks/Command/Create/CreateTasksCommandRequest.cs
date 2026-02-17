using FlashMediator;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Interfaces;

namespace ProjectManagement.Application.Features.CQRS.Tasks.Command.Create
{
    public record CreateTasksCommandRequest(string TaskName , string Description, DateTime DeadlineTime ) : IRequest
    {

    }
}
