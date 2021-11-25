using Microsoft.EntityFrameworkCore.Migrations;

namespace Anastock.Migrations.Anastock
{
    public partial class defaultUOM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UOM",
                table: "ProductAndService",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UOM",
                table: "ProductAndService");
        }
    }
}
