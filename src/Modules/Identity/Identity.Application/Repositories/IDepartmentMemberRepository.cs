using Identity.Domain.Entities;

namespace Identity.Application.Repositories;

public interface IDepartmentMemberRepository
{
    Task<DepartmentMember?> GetLeaderMembershipAsync(Guid userId, CancellationToken cancellationToken = default);
}
