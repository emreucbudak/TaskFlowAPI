using FlashMediator;
using TaskFlow.BuildingBlocks.Common;

namespace Report.Application.Features.CQRS.Reports.Query.GetAll
{
    public record GetAllReportsQueryRequest : IRequest<PagedResult<GetAllReportsQueryResponse>>
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public IReadOnlyCollection<Guid> ReportingUserIds { get; init; } = [];
    }
}
