using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        private readonly List<GroupsMember> _groupsMembers = new();

        private User()
        {
        }

        public User(string name, string email, Guid companyId)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
            if (companyId == Guid.Empty) throw new ArgumentException(nameof(companyId));

            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            UserName = email;
            CompanyId = companyId;
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public string Name { get; private set; }
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; }

        public Guid? DepartmentId { get; private set; }
        public Department? Department { get; private set; }

        public IReadOnlyCollection<GroupsMember> GroupsMembers => _groupsMembers.AsReadOnly();

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public void AssignToDepartment(Guid departmentId)
        {
            if (departmentId == Guid.Empty) throw new ArgumentException(nameof(departmentId));
            DepartmentId = departmentId;
        }

        public void RemoveFromDepartment()
        {
            DepartmentId = null;
        }

        public void AddGroupMember(GroupsMember member)
        {
            _groupsMembers.Add(member);
        }
    }
}