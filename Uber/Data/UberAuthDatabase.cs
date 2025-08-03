using Authnitication.Database;
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
        public UberAuthDatabase(DbContextOptions<AuthDatabase> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(DriverConfiguration).Assembly);

        }
    }
}
