using TaskFlow.BuildingBlocks.Common;

namespace Report.Domain.Entities
{
    public class ReportTopic : BaseEntity<int>
    {
        public string TopicName { get; set; }

        public static ReportTopic CreateSeed(int id, string topicName)
        {
            return new ReportTopic
            {
                Id = id,
                TopicName = topicName
            };
        }
    }
}
