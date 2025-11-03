using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);


            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.DiscountPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");







            var fixedDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                            // 🏺 Cups
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                                Name = "Ceramic Coffee Mug",
                                Description = "12oz ceramic mug perfect for coffee or tea.",
                                Price = 6.99m,
                                QuantityInStock = 120,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                                Name = "Travel Cup with Lid",
                                Description = "Stainless steel cup with lid, ideal for travel.",
                                Price = 14.50m,
                                QuantityInStock = 80,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },

                            // 📱 Electronics
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                                Name = "Wireless Mouse",
                                Description = "Ergonomic wireless mouse with USB receiver.",
                                Price = 19.99m,
                                QuantityInStock = 200,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                                Name = "Bluetooth Headphones",
                                Description = "Over-ear headphones with noise cancellation.",
                                Price = 59.99m,
                                DiscountPrice = 49.99m,
                                QuantityInStock = 75,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },

                            // 📚 Books
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                                Name = "The Great Gatsby",
                                Description = "Classic novel by F. Scott Fitzgerald.",
                                Price = 10.99m,
                                QuantityInStock = 50,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                                Name = "Atomic Habits",
                                Description = "A book about building good habits and breaking bad ones.",
                                Price = 16.99m,
                                QuantityInStock = 40,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },

                            // 👕 Clothing
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                                Name = "Men's T-Shirt",
                                Description = "100% cotton t-shirt, available in multiple colors.",
                                Price = 9.99m,
                                QuantityInStock = 300,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000008"),
                                Name = "Women's Hoodie",
                                Description = "Soft fleece hoodie for women.",
                                Price = 24.99m,
                                QuantityInStock = 150,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },

                            // 👟 Shoes
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000009"),
                                Name = "Running Shoes",
                                Description = "Lightweight running shoes for daily jogging.",
                                Price = 54.99m,
                                QuantityInStock = 90,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000010"),
                                Name = "Leather Formal Shoes",
                                Description = "Genuine leather shoes for formal occasions.",
                                Price = 79.99m,
                                QuantityInStock = 60,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },

                            // 🪑 Furniture
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000011"),
                                Name = "Office Chair",
                                Description = "Ergonomic mesh chair with adjustable height.",
                                Price = 129.99m,
                                QuantityInStock = 40,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000007"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000012"),
                                Name = "Wooden Coffee Table",
                                Description = "Modern wooden table for living rooms.",
                                Price = 89.99m,
                                QuantityInStock = 30,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000007"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },

                            // 🧴 Beauty
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000013"),
                                Name = "Face Moisturizer",
                                Description = "Hydrating daily moisturizer for all skin types.",
                                Price = 14.99m,
                                QuantityInStock = 120,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            },
                            new Product
                            {
                                Id = Guid.Parse("10000000-0000-0000-0000-000000000014"),
                                Name = "Lipstick Set",
                                Description = "Matte lipstick set with five shades.",
                                Price = 19.99m,
                                QuantityInStock = 70,
                                CategoryId = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                                ImageUrl = "",
                                CreatedAt = fixedDate
                            }
                        );
        }
    }
}
