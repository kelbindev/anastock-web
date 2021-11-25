using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Anastock.Migrations.Anastock
{
    public partial class invoicenullablelinkedquoteid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_LinkedQuoteId",
                table: "Invoices");

            migrationBuilder.AlterColumn<Guid>(
                name: "LinkedQuoteId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_LinkedQuoteId",
                table: "Invoices",
                column: "LinkedQuoteId",
                unique: true,
                filter: "[LinkedQuoteId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_LinkedQuoteId",
                table: "Invoices");

            migrationBuilder.AlterColumn<Guid>(
                name: "LinkedQuoteId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_LinkedQuoteId",
                table: "Invoices",
                column: "LinkedQuoteId",
                unique: true);
        }
    }
}
