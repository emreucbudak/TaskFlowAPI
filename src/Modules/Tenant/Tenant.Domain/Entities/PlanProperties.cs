using TaskFlow.BuildingBlocks.Common;

namespace Tenant.Domain.Entities
{
    public class PlanProperties : BaseEntity
    {
        public PlanProperties(int peopleAddedLimit, int teamLimit, int individualTaskLimit, bool isReportIncluded)
        {
            if (peopleAddedLimit < 0) throw new ArgumentException("Çalışan ekleme limiti negatif olamaz");
            if (teamLimit < 0) throw new ArgumentException("Takım ekleme limiti negatif olamaz");

            PeopleAddedLimit = peopleAddedLimit;

            TeamLimit = teamLimit;
            IndividualTaskLimit = individualTaskLimit;
            IsInternalReportingEnabled = isReportIncluded;
        }
        protected PlanProperties() { }

        public int PeopleAddedLimit { get; private set; }
        public int TeamLimit { get; private set; }
        public int IndividualTaskLimit { get; private set; }
        public bool IsInternalReportingEnabled { get; private set; }





    }
}
