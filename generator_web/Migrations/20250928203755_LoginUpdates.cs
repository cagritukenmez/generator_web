using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace generator_web.Migrations
{
    /// <inheritdoc />
    public partial class LoginUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Users",
                newName: "PasswordHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "name");
        }
    }
}
