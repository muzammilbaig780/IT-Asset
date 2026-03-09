using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class addeddeptcolumntelephone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Telephone",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Telephone_DepartmentId",
                table: "Telephone",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Telephone_Departments_DepartmentId",
                table: "Telephone",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Telephone_Departments_DepartmentId",
                table: "Telephone");

            migrationBuilder.DropIndex(
                name: "IX_Telephone_DepartmentId",
                table: "Telephone");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Telephone");
        }
    }
}
