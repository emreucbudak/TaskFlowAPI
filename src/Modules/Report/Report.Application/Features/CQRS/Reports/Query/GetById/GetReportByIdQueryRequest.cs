using FlashMediator;

namespace Report.Application.Features.CQRS.Reports.Query.GetById
{
    public record GetReportByIdQueryRequest(Guid Id) : IRequest<GetReportByIdQueryResponse>;
}
