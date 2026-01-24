namespace Identity.Domain.Entities
{
    public class GroupsMember 
    {
        public GroupsMember(Guid userId, Guid groupId, int groupRolesId)
        {
            UserId = userId;
            GroupId = groupId;
            GroupRolesId = groupRolesId;
        }

        public int GroupsMemberId { get;  set; }
        public Guid GroupId { get; private set; }
        public Groups Group { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public int GroupRolesId { get; private set; }
        public GroupRoles GroupRoles { get; private set; }

    }
}
