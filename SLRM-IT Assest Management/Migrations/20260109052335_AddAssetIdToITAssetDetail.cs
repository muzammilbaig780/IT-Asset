using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetIdToITAssetDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "ITAssetDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ComponentName",
                table: "ITAssetDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InstallDate",
                table: "ITAssetDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Accessories",
                columns: table => new
                {
                    AccessoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    AssignedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accessories", x => x.AccessoryId);
                    table.ForeignKey(
                        name: "FK_Accessories_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ITAssetDetails_AssetId",
                table: "ITAssetDetails",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Accessories_AssetId",
                table: "Accessories",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_ITAssetDetails_Assets_AssetId",
                table: "ITAssetDetails",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ITAssetDetails_Assets_AssetId",
                table: "ITAssetDetails");

            migrationBuilder.DropTable(
                name: "Accessories");

            migrationBuilder.DropIndex(
                name: "IX_ITAssetDetails_AssetId",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "ComponentName",
                table: "ITAssetDetails");

            migrationBuilder.DropColumn(
                name: "InstallDate",
                table: "ITAssetDetails");
        }
    }
}
