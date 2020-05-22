using Microsoft.EntityFrameworkCore.Migrations;

namespace MobileFueling.DB.Migrations
{
    public partial class addclientphonetoorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientOrderDetalizations_AspNetUsers_ClientId",
                table: "ClientOrderDetalizations");

            migrationBuilder.AlterColumn<long>(
                name: "ClientId",
                table: "ClientOrderDetalizations",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "ClientPhone",
                table: "ClientOrderDetalizations",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientOrderDetalizations_AspNetUsers_ClientId",
                table: "ClientOrderDetalizations",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientOrderDetalizations_AspNetUsers_ClientId",
                table: "ClientOrderDetalizations");

            migrationBuilder.DropColumn(
                name: "ClientPhone",
                table: "ClientOrderDetalizations");

            migrationBuilder.AlterColumn<long>(
                name: "ClientId",
                table: "ClientOrderDetalizations",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientOrderDetalizations_AspNetUsers_ClientId",
                table: "ClientOrderDetalizations",
                column: "ClientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
