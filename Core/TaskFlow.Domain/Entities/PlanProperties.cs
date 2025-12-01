using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class PlanProperties : BaseEntity
    {
        public int PeopleAddedLimit { get; set; }
        public bool isIncludeGroupChat { get; set; }
        public bool isIncludeVideoCall { get; set; }
        public int TeamLimit { get; set; }
        public bool isDailyPlannerEnabled { get; set; }
        public bool isIncludeTaskPriorityCategory { get; set; }
        public bool isDeadlineNotificationEnabled { get; set; }
        public Guid CompanyPlanId { get; set; }
        public CompanyPlan CompanyPlan { get; set; }
    }
}
