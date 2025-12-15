using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data
{
    public class IdentityManagementDbContext : IdentityDbContext<Identity.Domain.Entities.User, Identity.Domain.Entities.Roles, Guid>
    {
        public IdentityManagementDbContext(DbContextOptions options) : base(options)
        {
        }

        protected IdentityManagementDbContext()
        {
        }
        public DbSet<Identity.Domain.Entities.User> Users { get; set; }
        public DbSet<Identity.Domain.Entities.Roles> Roles { get; set; }
    }
}
