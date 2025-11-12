using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {

            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId)
                   .IsRequired();

            builder.Property(o => o.ShippingAddressId)
                   .IsRequired();

            builder.Property(o => o.CreatedAt)
                   .IsRequired()
                   .HasColumnType("timestamp with time zone")
                   .HasDefaultValueSql("NOW()");

            builder.Property(x => x.UpdatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(o => o.Status)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(o => o.Subtotal)
                   .IsRequired()
                   .HasColumnType("numeric(18,2)");

            builder.Property(o => o.ShippingCost)
                   .IsRequired()
                   .HasColumnType("numeric(18,2)");

            builder.Property(o => o.TotalAmount)
                   .IsRequired()
                   .HasColumnType("numeric(18,2)");

            builder.Property(o => o.ShippingMethod)
                   .HasMaxLength(100);

            builder.Property(o => o.PaymentMethod)
                   .HasMaxLength(100);

            builder.HasMany(o => o.Items)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
