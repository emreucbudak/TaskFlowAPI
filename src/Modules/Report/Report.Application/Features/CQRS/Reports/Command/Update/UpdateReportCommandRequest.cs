using FlashMediator;

namespace Report.Application.Features.CQRS.Reports.Command.Update
{
    public record UpdateReportCommandRequest : IRequest
    {
        public Guid Id { get; init; }
        public int? ReportTopicId { get; init; }
        public string? Description { get; init; }
        public int? ReportStatusId { get; init; }
        public string? Title { get; init; }
    }
}
