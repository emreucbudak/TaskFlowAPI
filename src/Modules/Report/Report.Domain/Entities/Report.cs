using TaskFlow.BuildingBlocks.Common;

namespace Report.Domain.Entities
{
    public class Report : BaseEntity
    {
        public Report(int reportTopicId, ReportTopic reportTopic, string description, Guid userId, int reportStatusId, string title)
        {
            ReportTopicId = reportTopicId;
            ReportTopic = reportTopic;
            Description = description;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            ReportStatusId = reportStatusId;
            Title = title;
        }

        public int ReportTopicId { get; private set; }
        public ReportTopic ReportTopic { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime CreatedAt { get; private set; } 
        public int ReportStatusId { get; private set; }
        public ReportStatus ReportStatus { get; private set; }
        public void UpdateReportStatus(int reportStatusId)
        {
            ReportStatusId = reportStatusId;
        }   
        public void UpdateReportTopic (int reportTopicId)
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
