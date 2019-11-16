using Microsoft.EntityFrameworkCore.Migrations;

namespace MobileFueling.DB.Migrations
{
    public partial class addregistrationnumberforcar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegNumber",
                table: "Cars",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegNumber",
                table: "Cars");
        }
    }
}
