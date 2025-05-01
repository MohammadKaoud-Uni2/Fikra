using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fikra.Migrations
{
    /// <inheritdoc />
    public partial class AnotherEditOnIdea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Ideas");
        }
    }
}
