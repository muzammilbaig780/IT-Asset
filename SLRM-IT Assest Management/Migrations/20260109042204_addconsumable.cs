using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class addconsumable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consumables",
                columns: table => new
                {
                    ConsumableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConsumableName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReorderLevel = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consumables", x => x.ConsumableId);
                });

            migrationBuilder.CreateTable(
                name: "ConsumableStocks",
                columns: table => new
                {
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    LastUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumableStocks", x => x.ConsumableId);
                    table.ForeignKey(
                        name: "FK_ConsumableStocks_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumableTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumableTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_ConsumableTransactions_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId");
                    table.ForeignKey(
                        name: "FK_ConsumableTransactions_Consumables_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumables",
                        principalColumn: "ConsumableId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumableTransactions_AssetId",
                table: "ConsumableTransactions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumableTransactions_ConsumableId",
                table: "ConsumableTransactions",
                column: "ConsumableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumableStocks");

            migrationBuilder.DropTable(
                name: "ConsumableTransactions");

            migrationBuilder.DropTable(
                name: "Consumables");
        }
    }
}
