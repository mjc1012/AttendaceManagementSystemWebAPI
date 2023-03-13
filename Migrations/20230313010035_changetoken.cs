using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendaceManagementSystemWebAPI.Migrations
{
    public partial class changetoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Employees",
                newName: "AccessToken");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "V/gIbGwpIJ1lwhj1UlFbg3lCQeeBZGQDlL1KFxfWh8s=");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccessToken",
                table: "Employees",
                newName: "Token");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "jP3HS2dmsEns0YN7fCyg5g7M2S6rPlpq6MQjdHXuobA=");
        }
    }
}
