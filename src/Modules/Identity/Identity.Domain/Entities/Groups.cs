namespace Identity.Domain.Entities
{
    public class Groups 
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public ICollection<GroupsMember> Users { get; private set; } = new List<GroupsMember>();
        public Groups(string name)
        {
            Name = name;
        }
        public void AddUser(Guid userid)
        {
            var groupMember = new GroupsMember(userid, this.Id);
            Users.Add(groupMember);
        }
    }
}
