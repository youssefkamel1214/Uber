using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Uber.Models.Domain.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            //builder.ToTable("Drivers");
            builder.Property(d => d.LicensePlate)
                .IsRequired().HasMaxLength(12);
            builder.Property(d => d.SSN)
                .IsRequired().HasMaxLength(14);
            builder.HasIndex(d => d.SSN)
                .IsUnique()
                .HasDatabaseName("IX_Drivers_SSN");
            builder.HasIndex(d => d.LicensePlate)
                .IsUnique()
                .HasDatabaseName("IX_Drivers_LicensePlate");
        }
    }
}
