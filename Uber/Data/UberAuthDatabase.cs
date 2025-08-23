using Authnitication.Database;
using Authnitication.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uber.Models.Domain;
using Uber.Models.Domain.Configurations;

namespace Uber.Data
{
    public class UberAuthDatabase: IdentityDbContext,IRefershTokenDataBase
    {
        public DbSet<UberUser> UberUsers { get; set; }
        public DbSet<Trip> trips { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<Cancellation> cancellations { get; set; }
        public DbSet<Tender>tenders { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public UberAuthDatabase(DbContextOptions<UberAuthDatabase> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(DriverConfiguration).Assembly);
            var driverRoleId = "59ed7de2-10a3-417d-9255-e83e16363d16";
            var passengerRoleId = "3848fb35-56df-4772-91ef-d5c16c005d79";
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id=driverRoleId,
                    ConcurrencyStamp = driverRoleId,
                    Name = "Driver",
                    NormalizedName = "Driver".ToUpper()
                },
                new IdentityRole
                {
                    Id=passengerRoleId,
                    ConcurrencyStamp = passengerRoleId,
                    Name = "Passenger",
                    NormalizedName = "Passenger".ToUpper()
                }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

        }
    }
}
