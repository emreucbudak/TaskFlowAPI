using TaskFlow.BuildingBlocks.Common;

namespace Identity.Domain.Entities
{
    public class DepartmentMember : BaseEntity<int>
    {
        public DepartmentMember(Guid userId, Guid departmentId, int departmentRoleId)
        {
            UserId = userId;
            DepartmentId = departmentId;
            DepartmentRoleId = departmentRoleId;
        }

        public Guid DepartmentId { get; private set; }
        public Department Department { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public int DepartmentRoleId { get; private set; }
        public DepartmentRole DepartmentRole { get; private set; }
    }
}
