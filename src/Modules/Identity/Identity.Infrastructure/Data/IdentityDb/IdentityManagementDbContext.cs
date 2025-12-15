using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data.IdentityDb
{
    public class IdentityManagementDbContext : IdentityDbContext<Domain.Entities.User, Domain.Entities.Roles, Guid>
    {
        public IdentityManagementDbContext(DbContextOptions options) : base(options)
        {
        }

        protected IdentityManagementDbContext()
        {
        }
        public DbSet<Domain.Entities.User> Users { get; set; }
        public DbSet<Domain.Entities.Roles> Roles { get; set; }
    }
}
