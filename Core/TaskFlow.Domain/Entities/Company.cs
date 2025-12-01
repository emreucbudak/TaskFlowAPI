using TaskFlow.Domain.Bases;

namespace TaskFlow.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string CompanyName { get; set; }
        public Guid CompanyPlanId { get; set; }
        public CompanyPlan CompanyPlan { get; set; }
    }
}
