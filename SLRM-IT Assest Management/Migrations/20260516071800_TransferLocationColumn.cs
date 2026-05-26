using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class TransferLocationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FromLocationId",
                table: "AssetTransferLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToLocationId",
                table: "AssetTransferLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransferLogs_FromLocationId",
                table: "AssetTransferLogs",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransferLogs_ToLocationId",
                table: "AssetTransferLogs",
                column: "ToLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransferLogs_AssetLocations_FromLocationId",
                table: "AssetTransferLogs",
                column: "FromLocationId",
                principalTable: "AssetLocations",
                principalColumn: "AssetLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransferLogs_AssetLocations_ToLocationId",
                table: "AssetTransferLogs",
                column: "ToLocationId",
                principalTable: "AssetLocations",
                principalColumn: "AssetLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransferLogs_AssetLocations_FromLocationId",
                table: "AssetTransferLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransferLogs_AssetLocations_ToLocationId",
                table: "AssetTransferLogs");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransferLogs_FromLocationId",
                table: "AssetTransferLogs");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransferLogs_ToLocationId",
                table: "AssetTransferLogs");

            migrationBuilder.DropColumn(
                name: "FromLocationId",
                table: "AssetTransferLogs");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                table: "AssetTransferLogs");
        }
    }
}
