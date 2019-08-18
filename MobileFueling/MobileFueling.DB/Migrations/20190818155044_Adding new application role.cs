using Microsoft.EntityFrameworkCore.Migrations;

namespace MobileFueling.DB.Migrations
{
    public partial class Addingnewapplicationrole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { 1L, "7750ee1f-06c8-4211-b9c0-633b8de6cd77", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L);
        }
    }
}
