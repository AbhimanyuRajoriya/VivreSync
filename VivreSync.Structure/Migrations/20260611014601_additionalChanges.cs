using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VivreSync.Structure.Migrations
{
    /// <inheritdoc />
    public partial class additionalChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELlSgBYcSePit7McKxJDosBgpmCJMsjoJL4HLnYsHdt7/c6/31EKYNbL/13y10Tp0g==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENuHLKsfU/8EjFQuJQ3Cqf6SqKIiHqLRG0nKimjRBeduQcPArwDn8DQhDFI3jxzY7w==");
        }
    }
}
