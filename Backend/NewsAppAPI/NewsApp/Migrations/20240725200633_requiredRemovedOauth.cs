using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsApp.Migrations
{
    public partial class requiredRemovedOauth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
           name: "OAuthToken",
           table: "Users",
           type: "nvarchar(max)",
           nullable: true,
           oldClrType: typeof(string),
           oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OAuthID",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
           name: "OAuthToken",
           table: "Users",
           type: "nvarchar(max)",
           nullable: false,
           defaultValue: "",
           oldClrType: typeof(string),
           oldType: "nvarchar(max)",
           oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OAuthID",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
