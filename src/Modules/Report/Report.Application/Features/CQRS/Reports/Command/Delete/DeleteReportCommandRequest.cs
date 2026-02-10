using FlashMediator;

namespace Report.Application.Features.CQRS.Reports.Command.Delete
{
    public record DeleteReportCommandRequest : IRequest
    {
        public Guid Id { get; init; }
    }
}
