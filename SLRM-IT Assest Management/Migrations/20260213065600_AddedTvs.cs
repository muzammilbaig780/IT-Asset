using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddedTvs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "Tv",
                columns: table => new
                {
                    TvId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetLocationId = table.Column<int>(type: "int", nullable: false),
                    TvSerialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrrenSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tv", x => x.TvId);
                    table.ForeignKey(
                        name: "FK_Tv_AssetLocations_AssetLocationId",
                        column: x => x.AssetLocationId,
                        principalTable: "AssetLocations",
                        principalColumn: "AssetLocationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tv_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tv_AssetLocationId",
                table: "Tv",
                column: "AssetLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Tv_DepartmentId",
                table: "Tv",
                column: "DepartmentId");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
               name: "Tv");
        }
    }
}
