using Authnitication.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uber.Models.Domain;
using Uber.Models.Domain.Configurations;

namespace Uber.Data
{
    public class UberAuthDatabase: AuthDatabase
    {
        public DbSet<Passenger> passengers { get; set; }
        public DbSet<Driver> drivers { get; set; }
        public DbSet<Trip> trips { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<Cancellation> cancellations { get; set; }
        public DbSet<Tender>tenders { get; set; }
        public UberAuthDatabase(DbContextOptions<AuthDatabase> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(DriverConfiguration).Assembly);
            var readerRoleId = "59ed7de2-10a3-417d-9255-e83e16363d16";
            var writerRoleId = "3848fb35-56df-4772-91ef-d5c16c005d79";
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id=readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Driver",
                    NormalizedName = "Driver".ToUpper()
                },
                new IdentityRole
                {
                    Id=writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "Passenger",
                    NormalizedName = "Passenger".ToUpper()
                }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

        }
    }
}
