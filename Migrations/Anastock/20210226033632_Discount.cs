using Microsoft.EntityFrameworkCore.Migrations;

namespace Anastock.Migrations.Anastock
{
    public partial class Discount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountNote",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "DiscountNote",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DiscountNote",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DiscountNote",
                table: "Bills");

            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Quotes",
                newName: "DiscountValue");

            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "PurchaseOrders",
                newName: "DiscountValue");

            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Invoices",
                newName: "DiscountValue");

            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Bills",
                newName: "DiscountValue");

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "Quotes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "PurchaseOrders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "Invoices",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountType",
                table: "Bills",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Bills");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "Quotes",
                newName: "DiscountPercent");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "PurchaseOrders",
                newName: "DiscountPercent");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "Invoices",
                newName: "DiscountPercent");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "Bills",
                newName: "DiscountPercent");

            migrationBuilder.AddColumn<string>(
                name: "DiscountNote",
                table: "Quotes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountNote",
                table: "PurchaseOrders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountNote",
                table: "Invoices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountNote",
                table: "Bills",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
