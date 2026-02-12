namespace Report.Application.Features.CQRS.Reports.Query.GetAll
{
    public record GetAllReportsQueryResponse
    {
        public GetAllReportsQueryResponse(Guid ıd, int reportTopicId, string title, string description, Guid reportingUserId, int reportStatusId, Guid notifiedDepartmantId, DateTime createdAt)
        {
            Id = ıd;
            ReportTopicId = reportTopicId;
            Title = title;
            Description = description;
            ReportingUserId = reportingUserId;
            ReportStatusId = reportStatusId;
            NotifiedDepartmantId = notifiedDepartmantId;
            CreatedAt = createdAt;
        }

        public Guid Id { get; init; }
        public int ReportTopicId { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public Guid ReportingUserId { get; init; }
        public int ReportStatusId { get; init; }
        public Guid NotifiedDepartmantId { get; init; }
        public DateTime CreatedAt { get; init; }
    }

}
