namespace Tenant.Domain.Entities
{
    public class CompanyPlan : TaskFlow.BuildingBlocks.Common.BaseEntity
    {
        public string PlanName { get; set; }
        public ICollection<PlanProperties> PlanProperties { get; set; }
    }
}
