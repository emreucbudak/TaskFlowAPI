using FlashMediator;
using Report.Application.Features.CQRS.Reports.Query.GetAll;
using TaskFlow.BuildingBlocks.Common;
using TaskFlow.BuildingBlocks.Interfaces;

namespace Report.Application.Features.CQRS.Reports.Query.GetByDepartment;

public sealed record GetDepartmentReportsQueryRequest : IRequest<PagedResult<GetAllReportsQueryResponse>>, ICacheableQuery
{
    public Guid DepartmentId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public string CacheKey => $"getdepartmentreports:{DepartmentId}:{Page}:{PageSize}";

    public TimeSpan? ExpirationTime => TimeSpan.FromMinutes(10);
}