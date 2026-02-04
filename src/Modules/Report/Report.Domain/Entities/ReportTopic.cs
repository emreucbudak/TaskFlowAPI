using TaskFlow.BuildingBlocks.Common;

namespace Report.Domain.Entities
{
    public class ReportTopic : BaseEntity<int>
    {
        public string TopicName { get; set; }
    }
}
