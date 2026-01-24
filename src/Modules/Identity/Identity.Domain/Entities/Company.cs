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
        private readonly List<Groups> _groups = new();
        private readonly List<Department> _departments = new();
        public IReadOnlyCollection<Groups> Groups => _groups;
        public IReadOnlyCollection<Department> Departments => _departments;
        public void AddGroup(Groups groups)
        {
            _groups.Add(groups);
        }
        public void removeGroup(Groups groups)
        {
            _groups.Remove(groups);
        }
        public void UpdateCompanyName(string companyName)
        {
            CompanyName = companyName;
        }
        public void AddDepartment(Department department)
        {
            _departments.Add(department);
        }
        public void RemoveDepartment(Department department)
        {
            _departments.Remove(department);
        }
    }
}
