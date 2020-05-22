using Microsoft.EntityFrameworkCore.Migrations;

namespace MobileFueling.DB.Migrations
{
    public partial class addidempotencekey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentIdempotenceKey",
                table: "ClientOrderDetalizations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIdempotenceKey",
                table: "ClientOrderDetalizations");
        }
    }
}
