using TaskFlow.BuildingBlocks.Common;

namespace Report.Domain.Entities
{
    public class Report : BaseEntity
    {
        public Report(int reportTopicId, ReportTopic reportTopic, string description, Guid userId)
        {
            ReportTopicId = reportTopicId;
            ReportTopic = reportTopic;
            Description = description;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        public int ReportTopicId { get; private set; }
        public ReportTopic ReportTopic { get; private set; }
        public string Description { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime CreatedAt { get; private set; } 


    }
}
