using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AssetTypeChangesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetTypes_TypeId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "AssetTypes",
                newName: "AssetTypeId");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "Assets",
                newName: "AssetTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_TypeId",
                table: "Assets",
                newName: "IX_Assets_AssetTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "SlNo",
                table: "Assets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetTypes_AssetTypeId",
                table: "Assets",
                column: "AssetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "AssetTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetTypes_AssetTypeId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "AssetTypes",
                newName: "TypeId");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "Assets",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assets_AssetTypeId",
                table: "Assets",
                newName: "IX_Assets_TypeId");

            migrationBuilder.AlterColumn<int>(
                name: "SlNo",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetTypes_TypeId",
                table: "Assets",
                column: "TypeId",
                principalTable: "AssetTypes",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
