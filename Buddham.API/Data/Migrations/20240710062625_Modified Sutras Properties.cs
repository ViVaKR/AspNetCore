using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buddham.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedSutrasProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sutra",
                table: "Sutras",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Translator",
                table: "Sutras",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sutra",
                table: "Sutras");

            migrationBuilder.DropColumn(
                name: "Translator",
                table: "Sutras");
        }
    }
}
