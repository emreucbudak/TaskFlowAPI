using TaskFlow.BuildingBlocks.Common;

namespace Report.Domain.Entities
{
    public class Report : BaseEntity
    {
        public Report(int reportTopicId, string description, Guid reportingUserId, int reportStatusId, string title, Guid notifiedDepartmantId)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));
            }

            if (reportingUserId == Guid.Empty)
            {
                throw new ArgumentException("Reporting user id cannot be empty.", nameof(reportingUserId));
            }

            ReportTopicId = reportTopicId;
            Description = description;
            ReportingUserId = reportingUserId;
            CreatedAt = DateTime.UtcNow;
            ReportStatusId = reportStatusId;
            Title = title;
            NotifiedDepartmantId = notifiedDepartmantId;
        }

        public int ReportTopicId { get; private set; }
        public ReportTopic ReportTopic { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid ReportingUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int ReportStatusId { get; private set; }
        public ReportStatus ReportStatus { get; private set; }
        public Guid NotifiedDepartmantId { get; private set; }

        public void UpdateReportStatus(int reportStatusId)
        {
            ReportStatusId = reportStatusId;
        }

        public void UpdateReportTopic(int reportTopicId)
        {
            ReportTopicId = reportTopicId;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }

        public void UpdateTitle(string title)
        {
            Title = title;
        }
    }
}
