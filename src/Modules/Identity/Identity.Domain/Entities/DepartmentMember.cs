namespace Identity.Domain.Entities
{
    public class DepartmentMember
    {
        public DepartmentMember(Guid userId, Guid departmentId, int departmentRoleId)
        {
            UserId = userId;
            DepartmentId = departmentId;
            DepartmentRoleId = departmentRoleId;
        }

        public int DepartmentMemberId { get; set; }
        public Guid DepartmentId { get; private set; }
        public Department Department { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public int DepartmentRoleId { get; private set; }
        public DepartmentRole DepartmentRole { get; private set; }
    }
}
