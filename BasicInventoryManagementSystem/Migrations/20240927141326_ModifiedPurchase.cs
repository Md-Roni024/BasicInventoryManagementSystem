using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasicInventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Supplier",
                table: "Purchases",
                newName: "Suppliere");

            migrationBuilder.RenameColumn(
                name: "PurchaseDate",
                table: "Purchases",
                newName: "UpdatedDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Purchases");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Purchases",
                newName: "PurchaseDate");

            migrationBuilder.RenameColumn(
                name: "Suppliere",
                table: "Purchases",
                newName: "Supplier");
        }
    }
}
