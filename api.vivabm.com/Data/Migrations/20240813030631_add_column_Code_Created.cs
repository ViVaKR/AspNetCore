using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViVaBM.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class add_column_Code_Created : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "created",
                table: "codes",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created",
                table: "codes");
        }
    }
}
