using FlashMediator;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Identity.Application.Features.CQRS.Groups.Command.Add
{
    public sealed record AddGroupsCommandRequest : IRequest, ILimitedQueryable
    {
        public string Name { get; init; } = string.Empty;
        public Guid CompanyId { get; init; }
        public Guid? DepartmentId { get; init; }
        public List<Guid> UserIds { get; init; } = [];

        public Guid TenantId => CompanyId;
        public LimitType limitType => LimitType.TeamLimit;
    }
}
