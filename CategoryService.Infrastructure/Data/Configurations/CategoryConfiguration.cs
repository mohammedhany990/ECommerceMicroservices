using CategoryService.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CategoryService.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(p => p.Id)
                 .HasColumnType("uuid")
                 .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);


            builder.Property(p => p.Description)
                .HasMaxLength(1000);






            builder.HasData(
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Cups", Description = "Various types of cups and mugs" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Electronics", Description = "Devices, gadgets, and accessories" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Books", Description = "Fiction, non-fiction, and educational books" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Clothing", Description = "Men's and women's fashion" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Shoes", Description = "Casual, formal, and sports shoes" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Name = "Beauty", Description = "Cosmetics, skincare, and beauty tools" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), Name = "Furniture", Description = "Home and office furniture" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), Name = "Groceries", Description = "Everyday food and household items" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), Name = "Toys", Description = "Toys and games for children" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Name = "Sports", Description = "Sports equipment and accessories" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Name = "Jewelry", Description = "Rings, necklaces, and bracelets" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Name = "Kitchen", Description = "Cookware, utensils, and appliances" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Name = "Garden", Description = "Plants, tools, and outdoor accessories" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Name = "Pet Supplies", Description = "Food, toys, and accessories for pets" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), Name = "Office", Description = "Office supplies and stationery" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000016"), Name = "Health", Description = "Health care and wellness products" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000017"), Name = "Automotive", Description = "Car parts and accessories" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000018"), Name = "Baby", Description = "Baby care products and toys" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000019"), Name = "Music", Description = "Instruments and accessories" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000020"), Name = "Movies", Description = "DVDs, Blu-rays, and collectibles" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000021"), Name = "Home Decor", Description = "Wall art, lighting, and home accessories" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000022"), Name = "Tools", Description = "Hand tools and power tools" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000023"), Name = "Gaming", Description = "Consoles, games, and accessories" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000024"), Name = "Travel", Description = "Luggage and travel accessories" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000025"), Name = "Art", Description = "Art supplies and materials" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000026"), Name = "Stationery", Description = "Notebooks, pens, and paper goods" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000027"), Name = "Cleaning", Description = "Cleaning tools and chemicals" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000028"), Name = "Outdoor", Description = "Camping and outdoor gear" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000029"), Name = "Crafts", Description = "DIY and crafting supplies" },
                new Category { Id = Guid.Parse("00000000-0000-0000-0000-000000000030"), Name = "Watches", Description = "Analog and digital watches" }
            );
        }
    }
}
