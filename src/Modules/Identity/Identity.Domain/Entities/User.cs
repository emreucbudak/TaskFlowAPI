using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        private readonly List<GroupsMember> _groupsMembers = new();

        private User()
        {
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public string Name { get; private set; }
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; }
        public IReadOnlyCollection<GroupsMember> GroupsMembers => _groupsMembers.AsReadOnly();
        private readonly List<DepartmentMember> _departmentMembers = new();
        public IReadOnlyCollection<DepartmentMember> DepartmentMembers => _departmentMembers.AsReadOnly();

        public static User Create(string name, string email, Guid companyId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email));
            if (companyId == Guid.Empty)
                throw new ArgumentException("Company ID cannot be empty", nameof(companyId));

            var user = new User
            {
                Name = name,
                Email = email,
                UserName = email,
                CompanyId = companyId,
                EmailConfirmed = false
            };

            return user;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public void AssignToDepartment(DepartmentMember member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (_departmentMembers.Any(x => x.DepartmentId == member.DepartmentId))
                throw new InvalidOperationException("User is already member of this department");

            _departmentMembers.Add(member);
        }

        public void RemoveFromDepartment(Guid departmentId)
        {
             var member = _departmentMembers.FirstOrDefault(x => x.DepartmentId == departmentId);
            if (member != null)
                _departmentMembers.Remove(member);
        }

        public void AddToGroup(GroupsMember member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (_groupsMembers.Any(x => x.GroupId == member.GroupId))
                throw new InvalidOperationException("User is already member of this group");

            _groupsMembers.Add(member);
        }

        public void RemoveFromGroup(Guid groupId)
        {
            var member = _groupsMembers.FirstOrDefault(x => x.GroupId == groupId);
            if (member != null)
                _groupsMembers.Remove(member);
        }
    }
}