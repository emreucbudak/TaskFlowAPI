using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.IDbContext
{
    public interface IIdentityManagementDbContext 
    {
       
        DbSet<User> Users { get; }
        DbSet<Groups> Groups { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
