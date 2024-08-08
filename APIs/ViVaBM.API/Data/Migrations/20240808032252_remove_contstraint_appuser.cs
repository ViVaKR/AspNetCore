using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ViVaBM.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class remove_contstraint_appuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_codes_AspNetUsers_app_user_id",
                table: "codes");

            migrationBuilder.DropIndex(
                name: "IX_codes_app_user_id",
                table: "codes");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "full_name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "refresh_token_expiry_time",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expiry_time",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_codes_app_user_id",
                table: "codes",
                column: "app_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_codes_AspNetUsers_app_user_id",
                table: "codes",
                column: "app_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
