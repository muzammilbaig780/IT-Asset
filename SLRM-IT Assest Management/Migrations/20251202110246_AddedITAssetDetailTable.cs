using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedITAssetDetailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ITAssetDetails",
                columns: table => new
                {
                    ITAssetDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    TelephoneNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ParallelConnection = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ScreenSize = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FrequencyNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LicenseNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ports = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITAssetDetails", x => x.ITAssetDetailId);
                    table.ForeignKey(
                        name: "FK_ITAssetDetails_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ITAssetDetails_AssetId",
                table: "ITAssetDetails",
                column: "AssetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ITAssetDetails");
        }
    }
}
