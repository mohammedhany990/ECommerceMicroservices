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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {

        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {


            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.ProductId)
                   .IsRequired();

            builder.Property(oi => oi.ProductName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(oi => oi.UnitPrice)
                   .IsRequired()
                   .HasColumnType("numeric(18,2)");

            builder.Property(oi => oi.Quantity)
                   .IsRequired();

            builder.Property(oi => oi.ImageUrl)
                   .HasMaxLength(500);

            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.Items)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
