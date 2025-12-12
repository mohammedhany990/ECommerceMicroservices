using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Enums;
using ShippingService.Domain.Entities;

namespace ShippingService.Infrastructure.Data.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.OrderId)
                   .HasDatabaseName("ix_shipments_order_id");

            builder.Property(x => x.OrderId)
                   .IsRequired();

            builder.Property(x => x.ShippingAddressId)
                   .IsRequired();

            builder.Property(x => x.ShippingMethodId)
                   .IsRequired();

            builder.Property(x => x.TrackingNumber)
                   .HasMaxLength(100)
                   .HasDefaultValue("TBD")
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .HasDefaultValue(ShipmentStatus.Pending)
                   .IsRequired();

            builder.Property(x => x.ShippingCost)
                   .IsRequired();

            builder.Property(x => x.ShippedAt)
                   .IsRequired()
                   .HasDefaultValueSql("NOW()");

            builder.Property(x => x.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("NOW()");

            builder.Property(x => x.UpdatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("NOW()");
        }
    }
}
