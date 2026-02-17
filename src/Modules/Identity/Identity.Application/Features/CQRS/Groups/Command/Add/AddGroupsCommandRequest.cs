using FlashMediator;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Groups.Command.Add
{
    public record AddGroupsCommandRequest(string Name, Guid companyId) : IRequest, ILimitedQueryable
    {
        public Guid TenantId => companyId;

        public LimitType limitType => LimitType.TeamLimit;
    }
}
