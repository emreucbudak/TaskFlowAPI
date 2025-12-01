using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class PlanProperties : BaseEntity
    {
        public int PeopleAddedLimit { get; set; }
        public bool IsIncludeGroupChat { get; set; }
        public bool IsIncludeVideoCall { get; set; }
        public int TeamLimit { get; set; }
        public bool IsDailyPlannerEnabled { get; set; }
        public bool IsIncludeTaskPriorityCategory { get; set; }
        public bool IsDeadlineNotificationEnabled { get; set; }
        public bool IsIncludeAddTaskNotifications { get; set; }
        public Guid CompanyPlanId { get; set; }
        public CompanyPlan CompanyPlan { get; set; }
    }
}
