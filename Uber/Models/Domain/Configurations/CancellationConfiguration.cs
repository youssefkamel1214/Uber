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
            builder.HasOne(c => c.User).WithMany().HasForeignKey(c => c.CancelledBy).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(c=>c.Trip)
                .WithMany()
                .HasForeignKey(c => c.TripId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(c => c.Fee).IsRequired();
           
            builder.HasIndex(c => c.TripId)
                .HasDatabaseName("IX_Cancellations_TripId");


        }
    }
}
