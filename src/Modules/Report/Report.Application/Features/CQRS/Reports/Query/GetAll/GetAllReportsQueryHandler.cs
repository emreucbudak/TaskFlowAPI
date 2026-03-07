using FlashMediator;
using Report.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;

namespace Report.Application.Features.CQRS.Reports.Query.GetAll
{
    public class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQueryRequest, PagedResult<GetAllReportsQueryResponse>>
    {
        private readonly IReportReadRepository _readRepository;

        public GetAllReportsQueryHandler(IReportReadRepository readRepository)
        {
            _readRepository = readRepository;
        }

        public async Task<PagedResult<GetAllReportsQueryResponse>> Handle(GetAllReportsQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _readRepository.GetAllAsync(
                request.PageSize,
                request.Page,
                false,
                request.ReportingUserIds);

            var mappedItems = result.Items.Select(r => new GetAllReportsQueryResponse(
                r.Id,
                r.ReportTopicId,
                r.Title,
                r.Description,
                r.ReportingUserId,
                r.ReportStatusId,
                r.NotifiedDepartmantId,
                r.CreatedAt
            )).ToList();

            return new PagedResult<GetAllReportsQueryResponse>
            {
                Items = mappedItems,
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
    }
}
