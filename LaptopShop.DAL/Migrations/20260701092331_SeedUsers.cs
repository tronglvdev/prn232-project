using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LaptopShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "Address", "Avatar", "Email", "FullName", "Password", "Phone", "RoleId" },
                values: new object[,]
                {
                    { 1L, "Hà Nội", "default.jpg", "admin@gmail.com", "Quản trị viên", "123", "0123456789", 1L },
                    { 2L, "Hồ Chí Minh", "default.jpg", "user@gmail.com", "Khách hàng", "123", "0987654321", 2L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: 2L);
        }
    }
}
