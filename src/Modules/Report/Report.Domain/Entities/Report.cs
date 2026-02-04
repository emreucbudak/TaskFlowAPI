using TaskFlow.BuildingBlocks.Common;

namespace Report.Domain.Entities
{
    public class Report : BaseEntity
    {
        public int ReportTopicId { get; set; }
        public ReportTopic ReportTopic { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public static Report Create(int topicId, string description, Guid userId)
        {
            return new Report
            {
                ReportTopicId = topicId,
                Description = description,
                UserId = userId
            };
        }
    }
}
