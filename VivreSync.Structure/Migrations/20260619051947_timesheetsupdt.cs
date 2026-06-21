using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VivreSync.Structure.Migrations
{
    /// <inheritdoc />
    public partial class timesheetsupdt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HoursWorked",
                table: "Timesheets",
                newName: "WednesdayHours");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityTag",
                table: "Timesheets",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "FridayHours",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayHours",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayHours",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Timesheets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SundayHours",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayHours",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuesdayHours",
                table: "Timesheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPUkx78MT9T9mvMDAaD471LXxR596D0c3wWbc9clFy0d5AVbWB9XVXd/51zk4/WdOA==");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FridayHours",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "MondayHours",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "SaturdayHours",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "SundayHours",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "ThursdayHours",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "TuesdayHours",
                table: "Timesheets");

            migrationBuilder.RenameColumn(
                name: "WednesdayHours",
                table: "Timesheets",
                newName: "HoursWorked");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ActivityTag",
                table: "Timesheets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAECVWaLv3XtW4+6z7Y1sbStg8juCSCqMCVVog47obXJy/suEV3Xup1+mkqfu3npHo1A==");
        }
    }
}
