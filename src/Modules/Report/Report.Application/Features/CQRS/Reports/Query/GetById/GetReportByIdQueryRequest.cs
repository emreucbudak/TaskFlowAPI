using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Report.Application.Features.CQRS.Reports.Query.GetById
{
    public record GetReportByIdQueryRequest : IRequest<GetReportByIdQueryResponse> , ICacheableQuery
    {
        public Guid ReportId { get; set; }

        public string CacheKey => "getreportbyid";

        public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(15);
    }
}
