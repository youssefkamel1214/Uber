using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Uber.Models.Domain.Configurations
{
    public class TenderConfiguration : IEntityTypeConfiguration<Tender>
    {
        public void Configure(EntityTypeBuilder<Tender> builder)
        {
            builder.HasKey(t => t.TenderId);
            builder.HasOne(t=>t.Trip)
                .WithMany()
                .HasForeignKey(t => t.TripId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(t=>t.Driver).WithMany()
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(t => t.OfferedPrice).IsRequired();
            builder.Property(t => t.TenderTime)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(t => t.staute).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.HasIndex(t => t.TripId);
            builder.HasIndex(t=>t.DriverId);

        }
    }
}
