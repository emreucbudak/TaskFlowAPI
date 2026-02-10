using FlashMediator;
using Report.Application.Features.CQRS.Reports.Query.GetAll;
using Report.Application.Features.CQRS.Reports.Query.GetById;
using Report.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;

namespace Report.Application.Features.CQRS.Reports.Query.GetAll
{
    public class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQueryRequest, PagedResult<GetReportByIdQueryResponse>>
    {
        private readonly IReportReadRepository _readRepository;

        public GetAllReportsQueryHandler(IReportReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<PagedResult<GetReportByIdQueryResponse>> Handle(GetAllReportsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _readRepository.GetAllAsync(request.PageSize, request.Page, false);

            var mappedItems = result.Items.Select(r => new GetReportByIdQueryResponse(
                r.Id,
                r.ReportTopicId,
                r.Title,
                r.Description,
                r.ReportingUserId,
                r.ReportStatusId,
                r.NotifiedDepartmantId,
                r.CreatedAt
            )).ToList();

            return new PagedResult<GetReportByIdQueryResponse>
            {
                Items = mappedItems,
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
    }
}
