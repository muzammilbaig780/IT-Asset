using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddConsumablesAndTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                table: "ConsumableTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "Consumables",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                table: "ConsumableTransactions");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Consumables");
        }
    }
}
