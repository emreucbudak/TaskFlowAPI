namespace Report.Application.Features.CQRS.Reports.Query.GetById
{
    public record GetReportByIdQueryResponse(
        Guid Id,
        int ReportTopicId,
        string Title,
        string Description,
        Guid ReportingUserId,
        int ReportStatusId,
        Guid NotifiedDepartmantId,
        DateTime CreatedAt
    );
}
