using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CategoryService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Various types of cups and mugs", "Cups" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Devices, gadgets, and accessories", "Electronics" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Fiction, non-fiction, and educational books", "Books" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Men's and women's fashion", "Clothing" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Casual, formal, and sports shoes", "Shoes" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Cosmetics, skincare, and beauty tools", "Beauty" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Home and office furniture", "Furniture" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Everyday food and household items", "Groceries" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Toys and games for children", "Toys" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Sports equipment and accessories", "Sports" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "Rings, necklaces, and bracelets", "Jewelry" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "Cookware, utensils, and appliances", "Kitchen" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "Plants, tools, and outdoor accessories", "Garden" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "Food, toys, and accessories for pets", "Pet Supplies" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "Office supplies and stationery", "Office" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "Health care and wellness products", "Health" },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "Car parts and accessories", "Automotive" },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "Baby care products and toys", "Baby" },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "Instruments and accessories", "Music" },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "DVDs, Blu-rays, and collectibles", "Movies" },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "Wall art, lighting, and home accessories", "Home Decor" },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "Hand tools and power tools", "Tools" },
                    { new Guid("00000000-0000-0000-0000-000000000023"), "Consoles, games, and accessories", "Gaming" },
                    { new Guid("00000000-0000-0000-0000-000000000024"), "Luggage and travel accessories", "Travel" },
                    { new Guid("00000000-0000-0000-0000-000000000025"), "Art supplies and materials", "Art" },
                    { new Guid("00000000-0000-0000-0000-000000000026"), "Notebooks, pens, and paper goods", "Stationery" },
                    { new Guid("00000000-0000-0000-0000-000000000027"), "Cleaning tools and chemicals", "Cleaning" },
                    { new Guid("00000000-0000-0000-0000-000000000028"), "Camping and outdoor gear", "Outdoor" },
                    { new Guid("00000000-0000-0000-0000-000000000029"), "DIY and crafting supplies", "Crafts" },
                    { new Guid("00000000-0000-0000-0000-000000000030"), "Analog and digital watches", "Watches" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
