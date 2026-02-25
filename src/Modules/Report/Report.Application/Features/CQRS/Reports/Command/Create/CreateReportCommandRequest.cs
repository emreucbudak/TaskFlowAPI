using FlashMediator;

namespace Report.Application.Features.CQRS.Reports.Command.Create
{
    public record CreateReportCommandRequest(int ReportTopicId, string Description, Guid UserId, int ReportStatusId, string Title, Guid NotifiedDepartmentId, Guid CompanyId) : IRequest
    {
    }
}
