using Microsoft.EntityFrameworkCore.Migrations;

namespace Anastock.Migrations.Anastock
{
    public partial class quoteinvrelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Invoices_QuoteId",
                table: "Quotes");

            migrationBuilder.RenameColumn(
                name: "QuoteId",
                table: "Invoices",
                newName: "LinkedQuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_LinkedQuoteId",
                table: "Invoices",
                column: "LinkedQuoteId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Quotes_LinkedQuoteId",
                table: "Invoices",
                column: "LinkedQuoteId",
                principalTable: "Quotes",
                principalColumn: "QuoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Quotes_LinkedQuoteId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_LinkedQuoteId",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "LinkedQuoteId",
                table: "Invoices",
                newName: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Invoices_QuoteId",
                table: "Quotes",
                column: "QuoteId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId");
        }
    }
}
