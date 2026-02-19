using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Features.UserRoles;
using Selu383.SP26.Api.Features.Roles;
using Selu383.SP26.Api.Features.Users;
using Selu383.SP26.Api.Features.Locations;

namespace Selu383.SP26.Api.Data
{

    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userRoleBuilder = modelBuilder.Entity<UserRole>();

            userRoleBuilder.HasKey(x => new { x.UserId, x.RoleId });

            userRoleBuilder.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId);

            userRoleBuilder.HasOne(x => x.User)
                .WithMany(navigationExpression: x => x.Roles)
                .HasForeignKey(x => x.UserId);
        }
           public DbSet<Location> Locations { get; set; }
    }
}