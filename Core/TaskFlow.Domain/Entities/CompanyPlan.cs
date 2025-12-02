using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class CompanyPlan : BaseEntity
    {
        public string PlanName { get; set; }
        public ICollection<PlanProperties> PlanProperties { get; set; }

    }
}
