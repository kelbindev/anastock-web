using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Anastock.Migrations.Anastock
{
    public partial class ProductBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductBalanceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Qty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedBillId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LinkedInvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBalanceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBalanceDetails_Bills_LinkedBillId",
                        column: x => x.LinkedBillId,
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBalanceDetails_Invoices_LinkedInvoiceId",
                        column: x => x.LinkedInvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBalanceDetails_ProductAndService_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ProductAndService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBalanceDetails_LinkedBillId",
                table: "ProductBalanceDetails",
                column: "LinkedBillId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBalanceDetails_LinkedInvoiceId",
                table: "ProductBalanceDetails",
                column: "LinkedInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBalanceDetails_ProductId",
                table: "ProductBalanceDetails",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductBalanceDetails");
        }
    }
}
