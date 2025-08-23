using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Uber.Models.Domain.Configurations
{
    public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        void IEntityTypeConfiguration<Passenger>.Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.OwnsOne(p => p.Home, h =>
            {
                h.Property(p => p.Longtitde).HasColumnName("HomeLongitude");
                h.Property(p => p.Latitude).HasColumnName("HomeLatitude");
            });
            builder.OwnsOne(p => p.Work, h =>
            {
                h.Property(p => p.Longtitde).HasColumnName("WorkLongitude");
                h.Property(p => p.Latitude).HasColumnName("WorkLatitude");
            });
        }
    }
}
