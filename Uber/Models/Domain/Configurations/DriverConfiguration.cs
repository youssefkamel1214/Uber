using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Uber.Models.Domain.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.DriverId);
            builder.HasOne(d => d.User).WithOne().HasForeignKey<Driver>(d => d.DriverId).OnDelete(DeleteBehavior.Cascade);
            builder.Property(d => d.FirstName)
                .IsRequired().HasMaxLength(100);
            builder.Property(d => d.LastName)
                .IsRequired().HasMaxLength(100);
            builder.Property(d => d.Email)
                .IsRequired().HasMaxLength(100);
            builder.Property(d => d.PhoneNumber)
                .IsRequired().HasMaxLength(12);
            builder.Property(d => d.LicensePlate)
                .IsRequired().HasMaxLength(12);
            builder.Property(d => d.SSN)
                .IsRequired().HasMaxLength(14);
            builder.HasIndex(d => d.Email)
                .IsUnique()
                .HasDatabaseName("IX_Drivers_Email");
            builder.HasIndex(d => d.PhoneNumber)
                .IsUnique()
                .HasDatabaseName("IX_Drivers_PhoneNumber");
            builder.HasIndex(d => d.SSN)
                .IsUnique()
                .HasDatabaseName("IX_Drivers_SSN");
            builder.HasIndex(d => d.LicensePlate)
                .IsUnique()
                .HasDatabaseName("IX_Drivers_LicensePlate");
            builder.Property(d => d.Rating).IsRequired(false);
        }
    }
}
