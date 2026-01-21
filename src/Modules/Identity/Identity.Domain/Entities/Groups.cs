namespace Identity.Domain.Entities
{
    public class Groups 
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        private List<GroupsMember> _users = new();
        public IReadOnlyCollection<GroupsMember> Users => _users;
        public Groups(string name)
        {
            Name = name;
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
