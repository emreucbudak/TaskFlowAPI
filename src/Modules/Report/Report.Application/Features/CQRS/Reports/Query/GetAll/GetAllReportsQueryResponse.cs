namespace Report.Application.Features.CQRS.Reports.Query.GetAll
{
    public record GetAllReportsQueryResponse(Guid Id,
        int ReportTopicId,
        string Title,
        string Description,
        Guid ReportingUserId,
        int ReportStatusId,
        Guid NotifiedDepartmantId,
        DateTime CreatedAt);

}
