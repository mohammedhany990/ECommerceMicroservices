using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    QuantityInStock = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DiscountPrice", "ImageUrl", "IsDeleted", "Name", "Price", "QuantityInStock", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "12oz ceramic mug perfect for coffee or tea.", null, "", false, "Ceramic Coffee Mug", 6.99m, 120, null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stainless steel cup with lid, ideal for travel.", null, "", false, "Travel Cup with Lid", 14.50m, 80, null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ergonomic wireless mouse with USB receiver.", null, "", false, "Wireless Mouse", 19.99m, 200, null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Over-ear headphones with noise cancellation.", 49.99m, "", false, "Bluetooth Headphones", 59.99m, 75, null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Classic novel by F. Scott Fitzgerald.", null, "", false, "The Great Gatsby", 10.99m, 50, null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A book about building good habits and breaking bad ones.", null, "", false, "Atomic Habits", 16.99m, 40, null },
                    { new Guid("10000000-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "100% cotton t-shirt, available in multiple colors.", null, "", false, "Men's T-Shirt", 9.99m, 300, null },
                    { new Guid("10000000-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Soft fleece hoodie for women.", null, "", false, "Women's Hoodie", 24.99m, 150, null },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lightweight running shoes for daily jogging.", null, "", false, "Running Shoes", 54.99m, 90, null },
                    { new Guid("10000000-0000-0000-0000-000000000010"), new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Genuine leather shoes for formal occasions.", null, "", false, "Leather Formal Shoes", 79.99m, 60, null },
                    { new Guid("10000000-0000-0000-0000-000000000011"), new Guid("00000000-0000-0000-0000-000000000007"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ergonomic mesh chair with adjustable height.", null, "", false, "Office Chair", 129.99m, 40, null },
                    { new Guid("10000000-0000-0000-0000-000000000012"), new Guid("00000000-0000-0000-0000-000000000007"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Modern wooden table for living rooms.", null, "", false, "Wooden Coffee Table", 89.99m, 30, null },
                    { new Guid("10000000-0000-0000-0000-000000000013"), new Guid("00000000-0000-0000-0000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hydrating daily moisturizer for all skin types.", null, "", false, "Face Moisturizer", 14.99m, 120, null },
                    { new Guid("10000000-0000-0000-0000-000000000014"), new Guid("00000000-0000-0000-0000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Matte lipstick set with five shades.", null, "", false, "Lipstick Set", 19.99m, 70, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
