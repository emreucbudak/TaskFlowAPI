using FlashMediator;
using TaskFlow.BuildingBlocks.Common;
using Report.Application.Features.CQRS.Reports.Query.GetById;

namespace Report.Application.Features.CQRS.Reports.Query.GetAll
{
    public record GetAllReportsQueryRequest(int Page = 1, int PageSize = 10) : IRequest<PagedResult<GetReportByIdQueryResponse>>;
}
