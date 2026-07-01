using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaptopShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class HashSeedPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "$2a$11$vsw8SPljVgZbM625IIPvMeT5TldMi4WlFY1VNxKfXR4DebXIlLpyG");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Password",
                value: "$2a$11$vsw8SPljVgZbM625IIPvMeT5TldMi4WlFY1VNxKfXR4DebXIlLpyG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "123");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Password",
                value: "123");
        }
    }
}
