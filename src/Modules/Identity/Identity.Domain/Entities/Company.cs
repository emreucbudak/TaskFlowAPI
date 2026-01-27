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
            if (groups is null)
            {
                throw new ArgumentNullException(nameof(groups), "Grup null olamaz.");
            }
            if (_groups.Any(g => g.Name == groups.Name))
            {
                throw new InvalidOperationException("Aynı isimde bir grup zaten mevcut.");
            }
            _groups.Add(groups);
        }
        public void removeGroup(Groups groups)
        {
            if(groups is null)
            {
                throw new ArgumentNullException(nameof(groups), "Grup null olamaz.");
            }
            if(!_groups.Contains(groups))
            {
                throw new InvalidOperationException("Grup bulunamadı.");
            }
            _groups.Remove(groups);
        }
        public void UpdateCompanyName(string companyName)
        {
            if(string.IsNullOrWhiteSpace(companyName))
            {
                throw new ArgumentException("Şirket ismi boş veya null olamaz.", nameof(companyName));
            }
            CompanyName = companyName;
        }
        public void AddDepartment(Department department)
        {
            if(department is null)
            {
                throw new ArgumentNullException(nameof(department), "Department null olamaz.");
            }
            if(_departments.Any(d => d.Name == department.Name))
            {
                throw new InvalidOperationException("Aynı isimde bir departman zaten mevcut.");
            }
            _departments.Add(department);
        }
        public void RemoveDepartment(Department department)
        {
            if(department is null)
            {
                throw new ArgumentNullException(nameof(department), "Department null olamaz.");
            }
            if(!_departments.Contains(department))
            {
                throw new InvalidOperationException("Departman bulunamadı.");
            }
            _departments.Remove(department);
        }
    }
}
