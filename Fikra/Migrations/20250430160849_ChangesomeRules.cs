using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fikra.Migrations
{
    /// <inheritdoc />
    public partial class ChangesomeRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedBudget",
                table: "Ideas");

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "IdeaRatings",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedBudget",
                table: "Ideas",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "IdeaRatings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
