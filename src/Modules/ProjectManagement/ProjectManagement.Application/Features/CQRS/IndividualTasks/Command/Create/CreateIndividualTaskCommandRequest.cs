using FlashMediator;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Interfaces;

namespace ProjectManagement.Application.Features.CQRS.IndividualTasks.Command.Create
{
    public record CreateIndividualTaskCommandRequest(
        Guid AssignedUserId,
        string TaskTitle,
        string Description,
        DateOnly Deadline,
        Guid CompanyId)
        : IRequest, ILimitedQueryable
    {
        public Guid TenantId => CompanyId;
        public LimitType limitType => LimitType.IndividualTask;
    }
}
