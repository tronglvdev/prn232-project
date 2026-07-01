using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LaptopShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "DetailDesc", "Factory", "Image", "Name", "Price", "Quantity", "ShortDesc", "Sold", "Target" },
                values: new object[,]
                {
                    { 1L, "Laptop Gaming ASUS ROG cấu hình mạnh", "ASUS", "1711078092373-asus-01.png", "Asus Gaming ROG", 20000000m, 100L, "Laptop Gaming ASUS ROG", 10L, "GAMING" },
                    { 2L, "Dell XPS 13 viền siêu mỏng", "DELL", "1711078452562-dell-01.png", "Dell XPS 13", 25000000m, 50L, "Laptop Doanh nhân cao cấp", 5L, "DOANH-NHAN" },
                    { 3L, "Lenovo Ideapad giá rẻ", "LENOVO", "1711079073759-lenovo-01.png", "Lenovo Ideapad", 12000000m, 200L, "Laptop học tập làm việc", 50L, "SINHVIEN-VANPHONG" },
                    { 4L, "Zenbook Duo tuyệt đỉnh", "ASUS", "1711079496409-asus-02.png", "Asus Zenbook Duo", 18000000m, 30L, "Laptop thiết kế 2 màn hình", 2L, "THIET-KE-DO-HOA" },
                    { 5L, "Macbook Air M1 tiết kiệm pin", "APPLE", "1711079954090-apple-01.png", "Macbook Air M1", 28000000m, 150L, "Macbook siêu mỏng", 40L, "MONG-NHE" },
                    { 6L, "LG Gram siêu nhẹ", "LG", "1711080386941-lg-01.png", "LG Gram 14", 22000000m, 80L, "Laptop nhẹ nhất", 15L, "MONG-NHE" },
                    { 7L, "Macbook Pro hiệu năng cao", "APPLE", "1711080787179-apple-02.png", "Macbook Pro M2", 30000000m, 60L, "Laptop Pro mạnh mẽ", 20L, "DOANH-NHAN" },
                    { 8L, "Acer Nitro 5 tản nhiệt mát", "ACER", "1711080948771-acer-01.png", "Acer Nitro 5", 16000000m, 120L, "Laptop Gaming giá tốt", 25L, "GAMING" },
                    { 9L, "Asus Vivobook trẻ trung", "ASUS", "1711081080930-asus-03.png", "Asus Vivobook", 14000000m, 150L, "Laptop mỏng đẹp", 30L, "SINHVIEN-VANPHONG" },
                    { 10L, "Dell Inspiron cho mọi nhà", "DELL", "1711081278418-dell-02.png", "Dell Inspiron", 9000000m, 250L, "Laptop Dell giá rẻ", 100L, "MONG-NHE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 10L);
        }
    }
}
