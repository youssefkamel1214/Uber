using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Uber.Models.Domain.Configurations
{
    public class CancellationConfiguration : IEntityTypeConfiguration<Cancellation>
    {
        public void Configure(EntityTypeBuilder<Cancellation> builder)
        {
            builder.HasKey(c => c.CancellationId);
            builder.Property(c => c.CancellationId)
                .ValueGeneratedOnAdd();
            builder.HasOne<Trip>()
                .WithOne()
                .HasForeignKey<Cancellation>(c => c.TripId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(c => c.TripId)
                .HasDatabaseName("IX_Cancellations_TripId");
            builder.Property(c=>c.cancelledBy).HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

        }
    }
}
