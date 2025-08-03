using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Uber.Models.Domain.Configurations
{
    public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        void IEntityTypeConfiguration<Passenger>.Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.HasKey(p => p.PassngerId);
            builder.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<Passenger>(p => p.PassngerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);
            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(12);
            builder.HasIndex(p => p.Email)
                .IsUnique()
                .HasDatabaseName("IX_Passengers_Email");
            builder.HasIndex(p => p.PhoneNumber)
                .IsUnique()
                .HasDatabaseName("IX_Passengers_PhoneNumber");
            builder.Property(p=>p.rating).IsRequired(false);
        }
    }
}
