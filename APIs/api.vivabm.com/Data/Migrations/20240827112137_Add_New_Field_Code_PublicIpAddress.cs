using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViVaBM.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_New_Field_Code_PublicIpAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "myip",
                table: "codes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "myip",
                table: "codes");
        }
    }
}
