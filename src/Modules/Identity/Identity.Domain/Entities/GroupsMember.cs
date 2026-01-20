namespace Identity.Domain.Entities
{
    public class GroupsMember 
    {
        public GroupsMember(Guid userId, int groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }

        public int GroupsMemberId { get;  set; }
        public int GroupId { get; private set; }
        public Groups Group { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; }

    }
}
