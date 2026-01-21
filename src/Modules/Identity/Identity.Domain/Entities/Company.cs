namespace Identity.Domain.Entities
{
    public class Company : TaskFlow.BuildingBlocks.Common.BaseEntity
    {
        public Company()
        {
        }

        public Company(string companyName)
        {
            CompanyName = companyName;
        }


        public string CompanyName { get; private set; }
        public ICollection<Groups> Groups { get; private set; } = new List<Groups>();
        public void AddGroup(Groups groups)
        {
            Groups.Add(groups);
        }
        public void removeGroup(Groups groups)
        {
            Groups.Remove(groups);
        }
        public void UpdateCompanyName(string companyName)
        {
            CompanyName = companyName;
        }
    }
}
