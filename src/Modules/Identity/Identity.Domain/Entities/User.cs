using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
        private readonly List<GroupsMember> _groupsMembers = new();
        public IReadOnlyCollection<GroupsMember> GroupsMembers => _groupsMembers;
        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
