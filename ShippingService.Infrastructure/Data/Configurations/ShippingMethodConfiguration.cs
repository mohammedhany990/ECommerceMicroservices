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
    public class ShippingMethodConfiguration : IEntityTypeConfiguration<ShippingMethod>
    {
        public void Configure(EntityTypeBuilder<ShippingMethod> builder)
        {

            builder.HasKey(x => x.Id);


            builder.Property(x => x.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.Cost)
                   .HasColumnType("decimal(10,2)")
                   .IsRequired();

            builder.Property(x => x.EstimatedDeliveryDays)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.UpdatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP")
                   .ValueGeneratedOnAddOrUpdate();



            builder.HasData(
                new ShippingMethod
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Standard Shipping",
                    Cost = 5.99m,
                    EstimatedDeliveryDays = 5,
                    IsActive = true,
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2025-11-11T00:00:00Z"), DateTimeKind.Utc),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2025-11-11T00:00:00Z"), DateTimeKind.Utc)
                },
                new ShippingMethod
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Express Shipping",
                    Cost = 12.99m,
                    EstimatedDeliveryDays = 2,
                    IsActive = true,
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2025-11-11T00:00:00Z"), DateTimeKind.Utc),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2025-11-11T00:00:00Z"), DateTimeKind.Utc)
                },
                new ShippingMethod
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Overnight Shipping",
                    Cost = 24.99m,
                    EstimatedDeliveryDays = 1,
                    IsActive = true,
                    CreatedAt = DateTime.SpecifyKind(DateTime.Parse("2025-11-11T00:00:00Z"), DateTimeKind.Utc),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.Parse("2025-11-11T00:00:00Z"), DateTimeKind.Utc)
                }
            );


        }
    }
}
