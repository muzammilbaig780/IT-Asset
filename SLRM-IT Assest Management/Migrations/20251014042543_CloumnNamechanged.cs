using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class CloumnNamechanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetStatuses_AssetStatusId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetTypes_AssetTypeId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "Assets",
                newName: "TypeId");

            migrationBuilder.RenameColumn(
                name: "AssetStatusId",
                table: "Assets",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_AssetTypeId",
                table: "Assets",
                newName: "IX_Assets_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_AssetStatusId",
                table: "Assets",
                newName: "IX_Assets_StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetStatuses_StatusId",
                table: "Assets",
                column: "StatusId",
                principalTable: "AssetStatuses",
                principalColumn: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetTypes_TypeId",
                table: "Assets",
                column: "TypeId",
                principalTable: "AssetTypes",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetStatuses_StatusId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetTypes_TypeId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Assets",
                newName: "AssetTypeId");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Assets",
                newName: "AssetStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_TypeId",
                table: "Assets",
                newName: "IX_Assets_AssetTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_StatusId",
                table: "Assets",
                newName: "IX_Assets_AssetStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetStatuses_AssetStatusId",
                table: "Assets",
                column: "AssetStatusId",
                principalTable: "AssetStatuses",
                principalColumn: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetTypes_AssetTypeId",
                table: "Assets",
                column: "AssetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
