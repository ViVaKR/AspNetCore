using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViVaBM.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_new_column_SubTitle_Code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "memo",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "sub_title",
                table: "codes",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sub_title",
                table: "codes");

            migrationBuilder.AddColumn<string>(
                name: "memo",
                table: "categories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
