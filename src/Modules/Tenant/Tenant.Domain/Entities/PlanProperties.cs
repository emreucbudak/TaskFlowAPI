namespace Tenant.Domain.Entities
{
    public class PlanProperties
    {
        public PlanProperties(int peopleAddedLimit, bool isIncludeGroupChat, int teamLimit, bool isDailyPlannerEnabled, bool isIncludeTaskPriorityCategory, bool isDeadlineNotificationEnabled, bool isIncludeAddTaskNotifications)
        {
            if(peopleAddedLimit <0)  throw new ArgumentException("Çalışan ekleme limiti negatif olamaz");
            if(teamLimit <0)  throw new ArgumentException("Takım ekleme limiti negatif olamaz");

            PeopleAddedLimit = peopleAddedLimit;
            IsIncludeGroupChat = isIncludeGroupChat;

            TeamLimit = teamLimit;
            IsDailyPlannerEnabled = isDailyPlannerEnabled;
            IsIncludeTaskPriorityCategory = isIncludeTaskPriorityCategory;
            IsDeadlineNotificationEnabled = isDeadlineNotificationEnabled;
            IsIncludeAddTaskNotifications = isIncludeAddTaskNotifications;
        }
        protected PlanProperties() { }

        public int PeopleAddedLimit { get; private set; }
        public bool IsIncludeGroupChat { get; private set; }
        public int TeamLimit { get; private set; }
        public bool IsDailyPlannerEnabled { get; private set; }
        public bool IsIncludeTaskPriorityCategory { get; private set; }
        public bool IsDeadlineNotificationEnabled { get; private set; }
        public bool IsIncludeAddTaskNotifications { get; private set; }

   
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
