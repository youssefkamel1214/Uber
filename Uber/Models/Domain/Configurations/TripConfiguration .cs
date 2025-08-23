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
            builder.HasOne(t=>t.Driver)
                .WithMany()
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull);
           builder.Property(t => t.DriverId)
                   .IsRequired(false);
            builder.HasOne(t=>t.Passenger)
                   .WithMany()
                   .HasForeignKey(t => t.PassengerId)
                   .OnDelete(DeleteBehavior.ClientSetNull);
            // Embed Source Point as columns
            builder.OwnsOne(t => t.Source, s =>
            {
                s.Property(p => p.Longtitde).HasColumnName("SourceLongitude").IsRequired(); // Fix typo: Longtitde -> Longitude
                s.Property(p => p.Latitude).HasColumnName("SourceLatitude").IsRequired();
            });

            // Embed Destination Point as columns
            builder.OwnsOne(t => t.Destination, d =>
            {
                d.Property(p => p.Longtitde).HasColumnName("DestinationLongitude").IsRequired();
                d.Property(p => p.Latitude).HasColumnName("DestinationLatitude").IsRequired();
            });
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
