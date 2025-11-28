using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Data.Configuration_s
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .ValueGeneratedOnAdd();

            builder.Property(n => n.UserId)
                .IsRequired();

            builder.Property(n => n.To)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(n => n.Subject)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(n => n.Body)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(n => n.IsSent)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.SentAt);

            builder.Property(n => n.ErrorMessage)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.HasIndex(n => n.UserId);
        }
    }
}
