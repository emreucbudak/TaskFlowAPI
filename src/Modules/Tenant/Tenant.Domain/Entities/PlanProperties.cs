namespace Tenant.Domain.Entities
{
    public class PlanProperties
    {
        public PlanProperties(int peopleAddedLimit, bool ısIncludeGroupChat, bool ısIncludeVideoCall, int teamLimit, bool ısDailyPlannerEnabled, bool ısIncludeTaskPriorityCategory, bool ısDeadlineNotificationEnabled, bool ısIncludeAddTaskNotifications)
        {
            if(peopleAddedLimit <0)  throw new ArgumentException("Çalışan ekleme limiti negatif olamaz");
            if(teamLimit <0)  throw new ArgumentException("Takım ekleme limiti negatif olamaz");

            PeopleAddedLimit = peopleAddedLimit;
            IsIncludeGroupChat = ısIncludeGroupChat;
            IsIncludeVideoCall = ısIncludeVideoCall;
            TeamLimit = teamLimit;
            IsDailyPlannerEnabled = ısDailyPlannerEnabled;
            IsIncludeTaskPriorityCategory = ısIncludeTaskPriorityCategory;
            IsDeadlineNotificationEnabled = ısDeadlineNotificationEnabled;
            IsIncludeAddTaskNotifications = ısIncludeAddTaskNotifications;
        }
        protected PlanProperties() { }

        public int PeopleAddedLimit { get; set; }
        public bool IsIncludeGroupChat { get; set; }
        public bool IsIncludeVideoCall { get; set; }
        public int TeamLimit { get; set; }
        public bool IsDailyPlannerEnabled { get; set; }
        public bool IsIncludeTaskPriorityCategory { get; set; }
        public bool IsDeadlineNotificationEnabled { get; set; }
        public bool IsIncludeAddTaskNotifications { get; set; }

   
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
