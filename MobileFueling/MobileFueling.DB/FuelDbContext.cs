using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobileFueling.Model;

namespace MobileFueling.DB
{
    public class FuelDbContext :
        IdentityDbContext<ApplicationUser, ApplicationRole, long, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        public FuelDbContext(DbContextOptions<FuelDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            FuelMobileBuilder.OnModelCreating(builder);
        }

        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<SystemAdmin> AdminUsers { get; set; }
        public DbSet<Driver> DriverUsers { get; set; }
        public DbSet<Client> ClientUsers { get; set; }
        public DbSet<Manager> ManagerUsers { get; set; }
    }
}