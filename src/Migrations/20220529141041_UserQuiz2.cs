using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnBotServer.Migrations
{
    public partial class UserQuiz2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompletedQuises",
                table: "CompletedQuises");

            migrationBuilder.RenameTable(
                name: "CompletedQuises",
                newName: "CompletedQuizzes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompletedQuizzes",
                table: "CompletedQuizzes",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompletedQuizzes",
                table: "CompletedQuizzes");

            migrationBuilder.RenameTable(
                name: "CompletedQuizzes",
                newName: "CompletedQuises");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompletedQuises",
                table: "CompletedQuises",
                column: "Id");
        }
    }
}
