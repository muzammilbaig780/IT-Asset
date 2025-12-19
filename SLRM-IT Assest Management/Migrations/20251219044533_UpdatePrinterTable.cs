using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePrinterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "PONumber",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "SlNo",
                table: "Printers");

            migrationBuilder.RenameColumn(
                name: "VendorName",
                table: "Printers",
                newName: "Warranty");

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Printers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GRNNumber",
                table: "Printers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ITAssetTag",
                table: "Printers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceDate",
                table: "Printers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Printers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Division",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "GRNNumber",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "ITAssetTag",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Printers");

            migrationBuilder.RenameColumn(
                name: "Warranty",
                table: "Printers",
                newName: "VendorName");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Printers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PONumber",
                table: "Printers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlNo",
                table: "Printers",
                type: "int",
                nullable: true);
        }
    }
}
