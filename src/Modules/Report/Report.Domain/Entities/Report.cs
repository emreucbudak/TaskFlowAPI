using TaskFlow.BuildingBlocks.Common;

namespace Report.Domain.Entities
{
    public class Report : BaseEntity
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public Guid RequesterUserId { get; private set; }
        public ReportType Type { get; private set; }

        private Report() { }

        public Report(string title, string content, Guid requesterUserId, ReportType type)
        {
            Title = title;
            Content = content;
            RequesterUserId = requesterUserId;
            Type = type;
            CreatedDate = DateTime.UtcNow;
        }

        public void UpdateContent(string newContent)
        {
            Content = newContent;
        }
    }

    public enum ReportType
    {
        Performance,
        TaskSummary,
        ProjectStatus
    }
}
