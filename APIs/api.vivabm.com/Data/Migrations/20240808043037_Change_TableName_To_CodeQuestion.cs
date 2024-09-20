using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViVaBM.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Change_TableName_To_CodeQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_code_categories_category_id",
                table: "code");

            migrationBuilder.DropPrimaryKey(
                name: "PK_code",
                table: "code");

            migrationBuilder.RenameTable(
                name: "code",
                newName: "code_question");

            migrationBuilder.RenameIndex(
                name: "IX_code_category_id",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_code_question_categories_category_id",
                table: "code_question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_code_question",
                table: "code_question");

            migrationBuilder.RenameTable(
                name: "code_question",
                newName: "code");

            migrationBuilder.RenameIndex(
                name: "IX_code_question_category_id",
                table: "code",
                newName: "IX_code_category_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_code",
                table: "code",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_code_categories_category_id",
                table: "code",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
