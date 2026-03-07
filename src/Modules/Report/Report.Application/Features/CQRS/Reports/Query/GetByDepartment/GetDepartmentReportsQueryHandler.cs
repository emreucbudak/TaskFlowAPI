using FlashMediator;
using Report.Application.Features.CQRS.Reports.Query.GetAll;
using Report.Application.Repositories;
using TaskFlow.BuildingBlocks.Common;

namespace Report.Application.Features.CQRS.Reports.Query.GetByDepartment;

public sealed class GetDepartmentReportsQueryHandler(IReportReadRepository readRepository)
    : IRequestHandler<GetDepartmentReportsQueryRequest, PagedResult<GetAllReportsQueryResponse>>
{
    public async Task<PagedResult<GetAllReportsQueryResponse>> Handle(GetDepartmentReportsQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await readRepository.GetByDepartmentAsync(request.DepartmentId, request.PageSize, request.Page, false);

        return new PagedResult<GetAllReportsQueryResponse>
        {
            Items = result.Items.Select(report => new GetAllReportsQueryResponse(
                report.Id,
                report.ReportTopicId,
                report.Title,
                report.Description,
                report.ReportingUserId,
                report.ReportStatusId,
                report.NotifiedDepartmantId,
                report.CreatedAt)).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }
}