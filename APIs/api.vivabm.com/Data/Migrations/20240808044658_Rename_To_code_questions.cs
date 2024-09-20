using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViVaBM.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Rename_To_code_questions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_code_question_categories_category_id",
                table: "code_question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_code_question",
                table: "code_question");

            migrationBuilder.RenameTable(
                name: "code_question",
                newName: "code_questions");

            migrationBuilder.RenameIndex(
                name: "IX_code_question_category_id",
                table: "code_questions",
                newName: "IX_code_questions_category_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_code_questions",
                table: "code_questions",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_code_questions_categories_category_id",
                table: "code_questions",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_code_questions_categories_category_id",
                table: "code_questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_code_questions",
                table: "code_questions");

            migrationBuilder.RenameTable(
                name: "code_questions",
                newName: "code_question");

            migrationBuilder.RenameIndex(
                name: "IX_code_questions_category_id",
                table: "code_question",
                newName: "IX_code_question_category_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_code_question",
                table: "code_question",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_code_question_categories_category_id",
                table: "code_question",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
