using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeControlSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCancelledToCourseOffering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "CourseOfferings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "CourseOfferings");
        }
    }
}
