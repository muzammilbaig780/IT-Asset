using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedCheckinandCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Returndate",
                table: "Components",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "AssetTransferLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckinDate",
                table: "Assets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckoutDate",
                table: "Assets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedOut",
                table: "Assets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Returndate",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "AssetTransferLogs");

            migrationBuilder.DropColumn(
                name: "CheckinDate",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CheckoutDate",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "IsCheckedOut",
                table: "Assets");
        }
    }
}
