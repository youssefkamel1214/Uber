using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Uber.Models.Domain.Configurations
{
    public class UberUserConfiguration : IEntityTypeConfiguration<UberUser>
    {
        public void Configure(EntityTypeBuilder<UberUser> builder)
        {
            builder.ToTable("AspNetUsers");
            builder
                .HasDiscriminator<string>("UserType")
                .HasValue<Driver>("Driver")
                .HasValue<Passenger>("Passenger");
        }
    }
}
