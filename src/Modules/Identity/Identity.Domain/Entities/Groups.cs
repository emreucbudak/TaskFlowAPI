using TaskFlow.BuildingBlocks.Common;

namespace Identity.Domain.Entities
{
    public class Groups : BaseEntity
    {

        public string Name { get; private set; }
        private List<GroupsMember> _users = new();
        public IReadOnlyCollection<GroupsMember> Users => _users;
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; }
        public Groups(string name, Guid companyId)
        {
            Name = name;
            CompanyId = companyId;

        }
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Group name cannot be empty.");
            }
            Name = newName;
        }
        public void AddUser(Guid userid,int rolesId)
        {
            var groupMember = new GroupsMember(userid, this.Id,rolesId);
            _users.Add(groupMember);
        }
        public void RemoveUser(Guid userid) {
            var userToRemove = Users.FirstOrDefault(u => u.UserId == userid);
            if (userToRemove is not null)
            {
                _users.Remove(userToRemove);
            }
        }


    }
}
