namespace Tenant.Domain.Entities
{
    public class CompanyPlan : TaskFlow.BuildingBlocks.Common.BaseEntity 
    {
        public string PlanName { get; set; }
        public PlanProperties PlanProperties { get; private set; }
        protected CompanyPlan() { }

        public CompanyPlan(string planName, PlanProperties planProperties)
        {
            PlanName = planName;
            PlanProperties = planProperties;
        }
        public void UpdateProperties(PlanProperties newProperties)
        {
            if (newProperties is null) throw new ArgumentNullException();
            PlanProperties = newProperties;
        }
        public PlanProperties GetPlanProperties()
        {
            return PlanProperties;
        }

    }
}
