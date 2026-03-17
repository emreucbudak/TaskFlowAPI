using Identity.Application.Repositories;
using Identity.Domain.Entities;
using Identity.Persistence.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence.Repositories;

public class DepartmentMemberRepository(IdentityManagementDbContext context) : IDepartmentMemberRepository
{
    private const int LeaderRoleId = 1;

    public async Task<DepartmentMember?> GetLeaderMembershipAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.DepartmentMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                dm => dm.UserId == userId && dm.DepartmentRoleId == LeaderRoleId,
                cancellationToken);
    }
}
