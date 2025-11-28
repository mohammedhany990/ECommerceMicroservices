using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

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

            builder.Property(o => o.UpdatedAt)
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

            builder.Property(o => o.ShippingMethodId)
                   .IsRequired();


            builder.Property(o => o.PaymentStatus)
                  .IsRequired()
                  .HasConversion<string>();

            builder.Property(o => o.PaymentId);

            builder.HasMany(o => o.Items)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
