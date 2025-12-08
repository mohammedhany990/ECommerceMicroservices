using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;

namespace ShippingService.Infrastructure.Data.Configurations
{
    public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
    {
        public void Configure(EntityTypeBuilder<ShippingAddress> builder)
        {

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId)
                   .HasDatabaseName("ix_shipping_addresses_user_id");


            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.FullName)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(x => x.AddressLine1)
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(x => x.AddressLine2)
                   .HasMaxLength(250);

            builder.Property(x => x.City)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.State)
                   .HasMaxLength(100);

            builder.Property(x => x.PostalCode)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.Country)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.IsDefault)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.UpdatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .ValueGeneratedOnAddOrUpdate();

            builder.HasIndex(x => new { x.UserId, x.IsDefault })
                   .HasDatabaseName("ix_shipping_addresses_user_default");
        }
    }
}
