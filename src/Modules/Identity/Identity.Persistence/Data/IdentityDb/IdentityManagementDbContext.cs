using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence.Data.IdentityDb
{
    public class IdentityManagementDbContext : IdentityDbContext<User, Roles, Guid>
    {
        public IdentityManagementDbContext(DbContextOptions<IdentityManagementDbContext> options) : base(options)
        {
        }

        protected IdentityManagementDbContext()
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<GroupsMember> GroupsMembers { get; set; }
        public DbSet<GroupRoles> GroupRoles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentRole> DepartmentRoles { get; set; }
        public DbSet<DepartmentMember> DepartmentMembers { get; set; }
        public DbSet<GroupActivity> GroupActivities { get; set; }
        public DbSet<GroupEvent> GroupEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Identity");
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityManagementDbContext).Assembly);
        }
    }
}
