using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAssets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetLocations_AssetLocationId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetStatuses_StatusId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Companies_CompanyId",
                table: "Assets");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SlNo",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssetLocationId",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetLocations_AssetLocationId",
                table: "Assets",
                column: "AssetLocationId",
                principalTable: "AssetLocations",
                principalColumn: "AssetLocationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetStatuses_StatusId",
                table: "Assets",
                column: "StatusId",
                principalTable: "AssetStatuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Companies_CompanyId",
                table: "Assets",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetLocations_AssetLocationId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetStatuses_StatusId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Companies_CompanyId",
                table: "Assets");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Assets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SlNo",
                table: "Assets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Assets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AssetLocationId",
                table: "Assets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetLocations_AssetLocationId",
                table: "Assets",
                column: "AssetLocationId",
                principalTable: "AssetLocations",
                principalColumn: "AssetLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetStatuses_StatusId",
                table: "Assets",
                column: "StatusId",
                principalTable: "AssetStatuses",
                principalColumn: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Companies_CompanyId",
                table: "Assets",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId");
        }
    }
}
