using TaskFlow.BuildingBlocks.Common;

namespace Report.Domain.Entities
{
    public class Report : BaseEntity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int ReportTopicId { get; private set; }
        public ReportTopic ReportTopic { get; private set; }
        public Guid ReportedByUserId { get; private set; }
        public Guid NotifiedDepartment { get; private set; }


    }
}
