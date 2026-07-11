using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeControlSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacultyStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Faculties",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Faculties");
        }
    }
}
