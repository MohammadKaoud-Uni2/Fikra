using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fikra.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTheIdea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedROI",
                table: "Ideas");

            migrationBuilder.RenameColumn(
                name: "ProfitPercentageOffered",
                table: "Ideas",
                newName: "EstimatedMonthlyRevenuePerUser");

            migrationBuilder.AlterColumn<string>(
                name: "ProblemStatement",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BigServerNeeded",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CompetitiveAdvantage",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeploymentFrequency",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ExpectedUserCount",
                table: "Ideas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Features",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "FrontendComplexity",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HaveBigFiles",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IdeaOwnerName",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RealisticConversionRate",
                table: "Ideas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresDevOpsSetup",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresRealTimeFeatures",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RetentionMonths",
                table: "Ideas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SecurityCriticalLevel",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tools",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BigServerNeeded",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "CompetitiveAdvantage",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "DeploymentFrequency",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "ExpectedUserCount",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "Features",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "FrontendComplexity",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "HaveBigFiles",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "IdeaOwnerName",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "RealisticConversionRate",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "RequiresDevOpsSetup",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "RequiresRealTimeFeatures",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "RetentionMonths",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "SecurityCriticalLevel",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "Tools",
                table: "Ideas");

            migrationBuilder.RenameColumn(
                name: "EstimatedMonthlyRevenuePerUser",
                table: "Ideas",
                newName: "ProfitPercentageOffered");

            migrationBuilder.AlterColumn<string>(
                name: "ProblemStatement",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "ExpectedROI",
                table: "Ideas",
                type: "float",
                nullable: true);
        }
    }
}
