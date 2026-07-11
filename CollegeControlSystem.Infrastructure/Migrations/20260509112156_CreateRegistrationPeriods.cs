using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeControlSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateRegistrationPeriods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrationPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Term = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationPeriods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPeriods_IsActive",
                table: "RegistrationPeriods",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationPeriods_Year_Term",
                table: "RegistrationPeriods",
                columns: new[] { "Year", "Term" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationPeriods");
        }
    }
}
