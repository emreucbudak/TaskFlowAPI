using FlashMediator;

namespace Report.Application.Features.CQRS.Reports.Command.Create
{
    public record CreateReportCommandRequest : IRequest
    {
        public int ReportTopicId { get; init; }
        public string Description { get; init; }
        public Guid UserId { get; init; }
        public int ReportStatusId { get; init; }
        public string Title { get; init; }
        public Guid NotifiedDepartmantId { get; init; }
    }
}
