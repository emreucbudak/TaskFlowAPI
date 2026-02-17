using FlashMediator;
using TaskFlow.BuildingBlocks.Enums;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Report.Application.Features.CQRS.Reports.Command.Create
{
    public record CreateReportCommandRequest(int ReportTopicId, string Description, Guid UserId, int ReportStatusId, string Title, Guid NotifiedDepartmentId, Guid CompanyId) : IRequest, ILimitedQueryable
    {
        public Guid TenantId => CompanyId;

        public LimitType limitType => LimitType.IsIncludeReporting;
    }
}
