using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedComponents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GRNNumber",
                table: "Barcode",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    ComponentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ModelNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetLocationId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Total = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remaining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalCost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckInCheckOut = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.ComponentId);
                    table.ForeignKey(
                        name: "FK_Components_AssetLocations_AssetLocationId",
                        column: x => x.AssetLocationId,
                        principalTable: "AssetLocations",
                        principalColumn: "AssetLocationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Components_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Components_AssetLocationId",
                table: "Components",
                column: "AssetLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_CategoryId",
                table: "Components",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "GRNNumber",
                table: "Barcode",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
