using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedAllITassetDetailColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssetLocation",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssetType",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Make",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNo",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlNo",
                table: "ITAssetDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetLocation",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "Division",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "Make",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "SerialNo",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "SlNo",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ITAssetDetails");
        }
    }
}
