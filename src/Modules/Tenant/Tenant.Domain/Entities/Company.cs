namespace Tenant.Domain.Entities
{
    public class Company : TaskFlow.BuildingBlocks.Common.BaseEntity
    {
        public string CompanyName { get; set; }
        public Guid CompanyPlanId { get; set; }
        public CompanyPlan CompanyPlan { get; set; }
    }
}
