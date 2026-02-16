using TaskFlow.BuildingBlocks.Common;

namespace Tenant.Domain.Entities
{
    public class PlanProperties : BaseEntity
    {
        public PlanProperties(int peopleAddedLimit, int teamLimit, bool isDailyPlannerEnabled, bool isDeadlineNotificationEnabled, int taskLimit, int individualTaskLimit)
        {
            if (peopleAddedLimit < 0) throw new ArgumentException("Çalışan ekleme limiti negatif olamaz");
            if (teamLimit < 0) throw new ArgumentException("Takım ekleme limiti negatif olamaz");

            PeopleAddedLimit = peopleAddedLimit;

            TeamLimit = teamLimit;
            IsDailyPlannerEnabled = isDailyPlannerEnabled;
            IsDeadlineNotificationEnabled = isDeadlineNotificationEnabled;
            TaskLimit = taskLimit;
            IndividualTaskLimit = individualTaskLimit;
        }
        protected PlanProperties() { }

        public int PeopleAddedLimit { get; private set; }
        public int TeamLimit { get; private set; }
        public int TaskLimit { get; private set; }
        public int IndividualTaskLimit { get; private set; }
        public bool IsDailyPlannerEnabled { get; private set; }
        public bool IsDeadlineNotificationEnabled { get; private set; }


   
        public bool CanAddNewUser(int currentPeopleCount)
        {
            return PeopleAddedLimit > currentPeopleCount;
        }
        public bool CanCreateNewTeam(int currentTeamCount)
        {
            return TeamLimit > currentTeamCount;
        }
    }
}
