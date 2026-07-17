using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedMasterColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "StockInventories");

            migrationBuilder.RenameColumn(
                name: "ItemName",
                table: "StockInventories",
                newName: "RequesitionNo");

            migrationBuilder.RenameColumn(
                name: "IssuedQty",
                table: "StockInventories",
                newName: "ItemNameMasterId");

            migrationBuilder.RenameColumn(
                name: "InvoiceNumber",
                table: "StockInventories",
                newName: "GRNNumber");

            migrationBuilder.AddColumn<int>(
                name: "ItemCodeMasterId",
                table: "StockInventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ItemCodeMasters",
                columns: table => new
                {
                    ItemCodeMasterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCodeMasters", x => x.ItemCodeMasterId);
                });

            migrationBuilder.CreateTable(
                name: "ItemNameMasters",
                columns: table => new
                {
                    ItemNameMasterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemNameMasters", x => x.ItemNameMasterId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockInventories_ItemCodeMasterId",
                table: "StockInventories",
                column: "ItemCodeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInventories_ItemNameMasterId",
                table: "StockInventories",
                column: "ItemNameMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockInventories_ItemCodeMasters_ItemCodeMasterId",
                table: "StockInventories",
                column: "ItemCodeMasterId",
                principalTable: "ItemCodeMasters",
                principalColumn: "ItemCodeMasterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockInventories_ItemNameMasters_ItemNameMasterId",
                table: "StockInventories",
                column: "ItemNameMasterId",
                principalTable: "ItemNameMasters",
                principalColumn: "ItemNameMasterId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockInventories_ItemCodeMasters_ItemCodeMasterId",
                table: "StockInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_StockInventories_ItemNameMasters_ItemNameMasterId",
                table: "StockInventories");

            migrationBuilder.DropTable(
                name: "ItemCodeMasters");

            migrationBuilder.DropTable(
                name: "ItemNameMasters");

            migrationBuilder.DropIndex(
                name: "IX_StockInventories_ItemCodeMasterId",
                table: "StockInventories");

            migrationBuilder.DropIndex(
                name: "IX_StockInventories_ItemNameMasterId",
                table: "StockInventories");

            migrationBuilder.DropColumn(
                name: "ItemCodeMasterId",
                table: "StockInventories");

            migrationBuilder.RenameColumn(
                name: "RequesitionNo",
                table: "StockInventories",
                newName: "ItemName");

            migrationBuilder.RenameColumn(
                name: "ItemNameMasterId",
                table: "StockInventories",
                newName: "IssuedQty");

            migrationBuilder.RenameColumn(
                name: "GRNNumber",
                table: "StockInventories",
                newName: "InvoiceNumber");

            migrationBuilder.AddColumn<DateOnly>(
                name: "InvoiceDate",
                table: "StockInventories",
                type: "date",
                nullable: true);
        }
    }
}
