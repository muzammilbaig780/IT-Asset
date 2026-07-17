using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class ForeignkeyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemNameMasterId",
                table: "ItemCodeMasters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemCodeMasters_ItemNameMasterId",
                table: "ItemCodeMasters",
                column: "ItemNameMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemCodeMasters_ItemNameMasters_ItemNameMasterId",
                table: "ItemCodeMasters",
                column: "ItemNameMasterId",
                principalTable: "ItemNameMasters",
                principalColumn: "ItemNameMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemCodeMasters_ItemNameMasters_ItemNameMasterId",
                table: "ItemCodeMasters");

            migrationBuilder.DropIndex(
                name: "IX_ItemCodeMasters_ItemNameMasterId",
                table: "ItemCodeMasters");

            migrationBuilder.DropColumn(
                name: "ItemNameMasterId",
                table: "ItemCodeMasters");
        }
    }
}
