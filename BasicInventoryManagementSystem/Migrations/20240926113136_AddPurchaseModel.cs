using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasicInventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Seller",
                table: "Purchases");

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "Purchases",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "Purchases");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Seller",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
