using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedTvVendorCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScrrenSize",
                table: "Tv",
                newName: "VendorName");

            migrationBuilder.AddColumn<string>(
                name: "Cost",
                table: "Tv",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpiryDate",
                table: "Tv",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseDate",
                table: "Tv",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScreenSize",
                table: "Tv",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Tv");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Tv");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Tv");

            migrationBuilder.DropColumn(
                name: "ScreenSize",
                table: "Tv");

            migrationBuilder.RenameColumn(
                name: "VendorName",
                table: "Tv",
                newName: "ScrrenSize");
        }
    }
}
