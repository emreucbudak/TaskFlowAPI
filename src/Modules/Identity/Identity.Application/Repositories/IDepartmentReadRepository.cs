namespace Identity.Application.Repositories
{
    public interface IDepartmentReadRepository
    {
        Task<Guid> GetDepartmentLeaderIdAsync(Guid departmentId);
    }
}
