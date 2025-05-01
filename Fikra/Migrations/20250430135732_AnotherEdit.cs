using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fikra.Migrations
{
    /// <inheritdoc />
    public partial class AnotherEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Ideas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
