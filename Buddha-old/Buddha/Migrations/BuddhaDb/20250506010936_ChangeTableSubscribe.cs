using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buddha.Migrations.BuddhaDb
{
    /// <inheritdoc />
    public partial class ChangeTableSubscribe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "subscribes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "subscribes");
        }
    }
}
