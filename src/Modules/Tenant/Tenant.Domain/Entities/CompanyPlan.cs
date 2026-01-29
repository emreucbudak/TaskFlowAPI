namespace Tenant.Domain.Entities
{
    public class CompanyPlan : TaskFlow.BuildingBlocks.Common.BaseEntity 
    {
        public string PlanName { get; private set; }
        public PlanProperties PlanProperties { get; private set; }
        public int PlanPrice { get; private set; }
        protected CompanyPlan() { }

        public CompanyPlan(string planName, PlanProperties planProperties, int planPrice)
        {
            PlanName = planName;
            PlanProperties = planProperties;
            PlanPrice = planPrice;
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
        public void UpdatePlanName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Plan ismi boş olamaz.");
            PlanName = newName;
        }
        public void UpdatePlanPrice(int newPrice)
        {
            if (newPrice < 0) throw new ArgumentException("Plan fiyatı negatif olamaz.");
            PlanPrice = newPrice;
        }

    }
}
