using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeControlSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeProgramNameUniquePerDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Programs_DepartmentId",
                table: "Programs");

            migrationBuilder.DropIndex(
                name: "IX_Programs_Name",
                table: "Programs");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_DepartmentId_Name",
                table: "Programs",
                columns: new[] { "DepartmentId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Programs_DepartmentId_Name",
                table: "Programs");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_DepartmentId",
                table: "Programs",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_Name",
                table: "Programs",
                column: "Name",
                unique: true);
        }
    }
}
