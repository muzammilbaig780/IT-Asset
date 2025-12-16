using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAssetinITassetDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ITAssetDetails_Assets_AssetId",
                table: "ITAssetDetails");

            migrationBuilder.DropIndex(
                name: "IX_ITAssetDetails_AssetId",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "ITAssetDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "ITAssetDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ITAssetDetails_AssetId",
                table: "ITAssetDetails",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_ITAssetDetails_Assets_AssetId",
                table: "ITAssetDetails",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId");
        }
    }
}
