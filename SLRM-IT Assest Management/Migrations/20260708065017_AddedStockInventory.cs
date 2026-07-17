using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedStockInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockInventories",
                columns: table => new
                {
                    StoreInventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReceivedQty = table.Column<int>(type: "int", nullable: false),
                    AvailableQty = table.Column<int>(type: "int", nullable: false),
                    IssuedQty = table.Column<int>(type: "int", nullable: false),
                    StoreLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInventories", x => x.StoreInventoryId);
                });

            migrationBuilder.CreateTable(
                name: "StockIssues",
                columns: table => new
                {
                    IssueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreInventoryId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueQty = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level1Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level1ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level1ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Level1Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level2Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level2ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level2ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Level2Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockIssues", x => x.IssueId);
                    table.ForeignKey(
                        name: "FK_StockIssues_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockIssues_StockInventories_StoreInventoryId",
                        column: x => x.StoreInventoryId,
                        principalTable: "StockInventories",
                        principalColumn: "StoreInventoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockIssues_DepartmentId",
                table: "StockIssues",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockIssues_StoreInventoryId",
                table: "StockIssues",
                column: "StoreInventoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockIssues");

            migrationBuilder.DropTable(
                name: "StockInventories");
        }
    }
}
