using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViVaBM.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Fk_Category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_codes_categories_category_id",
                table: "codes");

            migrationBuilder.DropIndex(
                name: "IX_codes_category_id",
                table: "codes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_codes_category_id",
                table: "codes",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "FK_codes_categories_category_id",
                table: "codes",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
