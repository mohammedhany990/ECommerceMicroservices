using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;
using Shared.Enums;


namespace PaymentService.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {


            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(p => p.OrderId)
                   .IsRequired();

            builder.Property(p => p.UserId)
                   .IsRequired();

            builder.Property(p => p.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.Currency)
                   .HasMaxLength(10)
                   .HasDefaultValue("usd")
                   .IsRequired();

            builder.Property(p => p.Status)
                   .IsRequired()
                   .HasConversion<int>();


            builder.Property(p => p.PaymentIntentId)
                   .HasMaxLength(100)
                   .HasDefaultValue(string.Empty);

            builder.Property(p => p.ClientSecret)
                   .HasMaxLength(200)
                   .HasDefaultValue(string.Empty);

            builder.Property(p => p.FailureReason)
                   .HasMaxLength(250);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.ConfirmedAt)
                   .IsRequired(false);
        }
    }
}
