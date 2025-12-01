using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class CompanyPlan : BaseEntity
    {
        public string PlanName { get; set; }
        public Guid PlanPropertiesId { get; set; }
        public PlanProperties PlanProperties { get; set; }

    }
}
