using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                   .IsRequired(false);

            builder.Property(x => x.Status)
                   .HasMaxLength(50)
                   .HasDefaultValue("Pending")
                   .IsRequired();
        }
    }
}
