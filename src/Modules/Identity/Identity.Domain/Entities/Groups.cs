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
                throw new ArgumentException("Grup adı boş olamaz");
            }
            Name = newName;
        }
        public void AddUser(Guid userid,int rolesId)
        {
            if(userid == Guid.Empty)
            {
                throw new ArgumentException("Çalışan idsi boş olamaz");
            }
            if(_users.Any(u => u.UserId == userid))
            {
                throw new InvalidOperationException("Çalışan zaten gruba üye");
            }
            var groupMember = new GroupsMember(userid, this.Id,rolesId);
            _users.Add(groupMember);
        }
        public void RemoveUser(Guid userid)
        {
            if (_users.Any(u => u.UserId == userid) is false)
            {
                throw new InvalidOperationException("Çalışan bu grubun üyesi değil");
            }
            var userToRemove = Users.FirstOrDefault(u => u.UserId == userid);
            if (userToRemove is not null)
            {
                _users.Remove(userToRemove);
            }
        }


    }
}
