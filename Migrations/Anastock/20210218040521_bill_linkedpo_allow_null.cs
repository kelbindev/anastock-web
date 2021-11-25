using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Anastock.Migrations.Anastock
{
    public partial class bill_linkedpo_allow_null : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bills_LinkedPOId",
                table: "Bills");

            migrationBuilder.AlterColumn<Guid>(
                name: "LinkedPOId",
                table: "Bills",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_LinkedPOId",
                table: "Bills",
                column: "LinkedPOId",
                unique: true,
                filter: "[LinkedPOId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bills_LinkedPOId",
                table: "Bills");

            migrationBuilder.AlterColumn<Guid>(
                name: "LinkedPOId",
                table: "Bills",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bills_LinkedPOId",
                table: "Bills",
                column: "LinkedPOId",
                unique: true);
        }
    }
}
