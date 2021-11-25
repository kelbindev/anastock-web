using Microsoft.EntityFrameworkCore.Migrations;

namespace Anastock.Migrations.Anastock
{
    public partial class bill_po_relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bills_LinkedPOId",
                table: "Bills");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_LinkedPOId",
                table: "Bills",
                column: "LinkedPOId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bills_LinkedPOId",
                table: "Bills");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_LinkedPOId",
                table: "Bills",
                column: "LinkedPOId",
                unique: true,
                filter: "[LinkedPOId] IS NOT NULL");
        }
    }
}
