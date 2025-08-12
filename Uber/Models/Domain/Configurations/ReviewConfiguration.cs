using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Uber.Models.Domain.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.ReviewId);
            builder.Property(r => r.ReviewId)
                .ValueGeneratedOnAdd();
            builder.HasOne<Trip>()
                .WithMany()
                .HasForeignKey(r => r.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.type)
                .HasConversion<string>().HasMaxLength(50)
                    .IsRequired();
            builder.HasIndex(r => r.TripId)
                .HasDatabaseName("IX_Reviews_TripId");
        }
    }
}
