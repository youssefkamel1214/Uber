using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Uber.Models.Domain.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.TripId);
            builder.Property(t => t.TripId)
                .ValueGeneratedOnAdd();
            builder.HasOne<Driver>()
                .WithMany()
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull);
           builder.Property(t => t.DriverId)
                   .IsRequired(false);
            builder.HasOne<Passenger>()
                   .WithMany()
                   .HasForeignKey(t => t.PassengerId)
                   .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Property(t => t.PassengerId)
                 .IsRequired(false);
            builder.Property(t=>t.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.HasIndex(builder => builder.DriverId)
                .HasDatabaseName("IX_Trips_DriverId");
            builder.HasIndex(builder => builder.PassengerId)
                   .HasDatabaseName("IX_Trips_PassengerId");
            
        }
    }
}
