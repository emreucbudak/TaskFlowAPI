using TaskFlow.BuildingBlocks.Common;

namespace Identity.Domain.Entities
{
    public class Department : BaseEntity
    {
        public Department(string name, Guid companyId)
        {
            Name = name;
            CompanyId = companyId;
        }

        public string Name { get; private set; }
        public Guid CompanyId { get; private set; }
        private readonly List<DepartmentMember> _departmentMembers = new();
        public IReadOnlyCollection<DepartmentMember> DepartmentMembers => _departmentMembers.AsReadOnly();
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Departman ismi boş veya null olamaz.");
            }
            Name = newName;
        }
        public void AddUser(Guid userId, int roleId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Çalışan idsi boş olamaz.", nameof(userId));
            }
            if(_departmentMembers.Any(u => u.UserId == userId))
            {
                throw new InvalidOperationException("Çalışan zaten departmana üye.");
            }
            var member = new DepartmentMember(userId, this.Id, roleId);
            _departmentMembers.Add(member);
        }

        public void RemoveUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Çalışan idsi boş olamaz.", nameof(userId));
            }
            var member = _departmentMembers.FirstOrDefault(u => u.UserId == userId);
            if (member == null)
            {
                throw new InvalidOperationException("Çalışan bu departmanın üyesi değil.");
            }
            _departmentMembers.Remove(member);
        }


    }
}
