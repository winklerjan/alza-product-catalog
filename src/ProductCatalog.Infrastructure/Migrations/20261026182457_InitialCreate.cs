using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductCatalog.Infrastructure.Migrations
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
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImgUri = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImgUri", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Thin and light laptop with all-day battery life.", "https://placehold.co/600x400?text=MacBook+Air", true, "Apple MacBook Air 13\" M4", 29990.00m },
                    { 2, "Compact Windows ultrabook with a 13-inch InfinityEdge display.", "https://placehold.co/600x400?text=Dell+XPS+13", true, "Dell XPS 13", 27490.00m },
                    { 3, null, "https://placehold.co/600x400?text=ThinkPad+X1", false, "Lenovo ThinkPad X1 Carbon", 34990.00m },
                    { 4, "14-inch gaming laptop with a dedicated RTX GPU.", "https://placehold.co/600x400?text=ROG+Zephyrus", true, "ASUS ROG Zephyrus G14", 42990.00m },
                    { 5, "Convertible laptop with a 360-degree hinge and OLED display.", "https://placehold.co/600x400?text=Spectre+x360", true, "HP Spectre x360", 31990.00m },
                    { 6, "Flagship smartphone with the A18 chip and dual camera system.", "https://placehold.co/600x400?text=iPhone+16", true, "Apple iPhone 16", 23990.00m },
                    { 7, null, "https://placehold.co/600x400?text=Galaxy+S25", true, "Samsung Galaxy S25", 21990.00m },
                    { 8, "Stock Android phone with computational photography.", "https://placehold.co/600x400?text=Pixel+9", true, "Google Pixel 9", 18990.00m },
                    { 9, "Mid-range smartphone with a 120Hz AMOLED display.", "https://placehold.co/600x400?text=Galaxy+A55", true, "Samsung Galaxy A55", 8990.00m },
                    { 10, null, "https://placehold.co/600x400?text=iPhone+SE", true, "Apple iPhone SE", 12990.00m },
                    { 11, "Over-ear noise-cancelling headphones with 30-hour battery life.", "https://placehold.co/600x400?text=Sony+WH-1000XM5", true, "Sony WH-1000XM5", 8490.00m },
                    { 12, "In-ear earbuds with active noise cancellation.", "https://placehold.co/600x400?text=AirPods+Pro+2", true, "Apple AirPods Pro 2", 6490.00m },
                    { 13, null, "https://placehold.co/600x400?text=Bose+QC+Ultra", false, "Bose QuietComfort Ultra", 9490.00m },
                    { 14, "Audiophile-tuned wireless headphones with a 60-hour battery.", "https://placehold.co/600x400?text=Momentum+4", true, "Sennheiser Momentum 4", 7990.00m },
                    { 15, "Budget-friendly noise-cancelling headphones.", "https://placehold.co/600x400?text=JBL+Tune+760NC", true, "JBL Tune 760NC", 1990.00m },
                    { 16, "27-inch 4K gaming monitor with a 144Hz refresh rate.", "https://placehold.co/600x400?text=UltraGear+27GP950", true, "LG UltraGear 27GP950", 15990.00m },
                    { 17, null, "https://placehold.co/600x400?text=UltraSharp+U2723QE", true, "Dell UltraSharp U2723QE", 13990.00m },
                    { 18, "32-inch curved gaming monitor with a 240Hz refresh rate.", "https://placehold.co/600x400?text=Odyssey+G7", true, "Samsung Odyssey G7", 12990.00m },
                    { 19, "27-inch monitor calibrated for color-accurate work.", "https://placehold.co/600x400?text=ProArt+PA278CV", true, "ASUS ProArt PA278CV", 9990.00m },
                    { 20, null, "https://placehold.co/600x400?text=BenQ+GW2780", false, "BenQ GW2780", 4490.00m },
                    { 21, "Ergonomic wireless mouse with an 8K DPI sensor.", "https://placehold.co/600x400?text=MX+Master+3S", true, "Logitech MX Master 3S", 2490.00m },
                    { 22, "Hot-swappable mechanical keyboard with wireless connectivity.", "https://placehold.co/600x400?text=Keychron+K8+Pro", true, "Keychron K8 Pro", 3290.00m },
                    { 23, null, "https://placehold.co/600x400?text=Logitech+C920", true, "Logitech C920 HD Pro", 1790.00m },
                    { 24, "20,000mAh portable charger with fast charging support.", "https://placehold.co/600x400?text=PowerCore+20000", true, "Anker PowerCore 20000", 1290.00m },
                    { 25, "Smartwatch with health tracking and always-on display.", "https://placehold.co/600x400?text=Apple+Watch+S10", true, "Apple Watch Series 10", 11990.00m }
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
