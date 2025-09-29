using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace generator_web.Migrations
{
    /// <inheritdoc />
    public partial class Test43 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "baterya",
                table: "generator_datas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "baterya",
                table: "generator_datas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
