using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SLRM_IT_Assest_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentIdToUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "UserProfile");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "UserProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_DepartmentId",
                table: "UserProfile",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Departments_DepartmentId",
                table: "UserProfile",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Departments_DepartmentId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_DepartmentId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "UserProfile");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
