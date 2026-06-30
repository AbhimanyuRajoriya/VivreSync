using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VivreSync.Structure.Migrations
{
    /// <inheritdoc />
    public partial class tokenversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TokenVersion",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "TokenVersion" },
                values: new object[] { "AQAAAAIAAYagAAAAEIbZOQT+N2dO9EzMeZg8Y8oTpyS2RFrUlwchH2e1TSy/ncFjiRDWYYTVk5xZodCoUg==", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenVersion",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJGwz6LQSn+KNh4twacGU36VSs/TjfSTvaiOYRZ/pa8ZZPdt6aXEabJVClhvyGfI7Q==");
        }
    }
}
