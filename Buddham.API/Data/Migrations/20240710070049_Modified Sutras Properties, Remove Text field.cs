using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buddham.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedSutrasPropertiesRemoveTextfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Sutras");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Sutras",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Sutras",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Sutras");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Sutras");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Sutras",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
