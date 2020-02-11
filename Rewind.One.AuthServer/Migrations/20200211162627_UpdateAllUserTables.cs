using Microsoft.EntityFrameworkCore.Migrations;

namespace Rewind.One.AuthServer.Migrations
{
    public partial class UpdateAllUserTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Users_AppUserId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Logins_Users_AppUserId",
                table: "Logins");

            migrationBuilder.DropIndex(
                name: "IX_Logins_AppUserId",
                table: "Logins");

            migrationBuilder.DropIndex(
                name: "IX_Claims_AppUserId",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Claims");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Users_UserId",
                table: "Claims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Logins_Users_UserId",
                table: "Logins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Users_UserId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Logins_Users_UserId",
                table: "Logins");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Logins",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Claims",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logins_AppUserId",
                table: "Logins",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_AppUserId",
                table: "Claims",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Users_AppUserId",
                table: "Claims",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Logins_Users_AppUserId",
                table: "Logins",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
