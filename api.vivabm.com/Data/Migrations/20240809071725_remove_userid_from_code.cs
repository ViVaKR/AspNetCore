using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViVaBM.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class remove_userid_from_code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "app_user_id",
                table: "codes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "app_user_id",
                table: "codes",
                type: "character varying(450)",
                maxLength: 450,
                nullable: true);
        }
    }
}
