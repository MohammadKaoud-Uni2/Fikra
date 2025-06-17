using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fikra.Migrations
{
    /// <inheritdoc />
    public partial class TryAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CVs_AspNetUsers_ApplicationUserId",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillLevel_CVs_CVId",
                table: "SkillLevel");

            migrationBuilder.AlterColumn<string>(
                name: "CVId",
                table: "SkillLevel",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdeaTitle",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdeaTitle",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Drafts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdeaTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdeaOwnerPercentage = table.Column<double>(type: "float", nullable: false),
                    Budget = table.Column<double>(type: "float", nullable: false),
                    Statue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drafts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "moneyTransferRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReceiverUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Statue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moneyTransferRequests", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_AspNetUsers_ApplicationUserId",
                table: "CVs",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillLevel_CVs_CVId",
                table: "SkillLevel",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CVs_AspNetUsers_ApplicationUserId",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillLevel_CVs_CVId",
                table: "SkillLevel");

            migrationBuilder.DropTable(
                name: "Drafts");

            migrationBuilder.DropTable(
                name: "moneyTransferRequests");

            migrationBuilder.DropColumn(
                name: "IdeaTitle",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "IdeaTitle",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "CVId",
                table: "SkillLevel",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_AspNetUsers_ApplicationUserId",
                table: "CVs",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillLevel_CVs_CVId",
                table: "SkillLevel",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id");
        }
    }
}
