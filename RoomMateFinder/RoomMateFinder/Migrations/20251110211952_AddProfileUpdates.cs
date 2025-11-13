using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomMateFinder.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnboarded",
                table: "Profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OnboardedAt",
                table: "Profiles",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnboarded",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "OnboardedAt",
                table: "Profiles");
        }
    }
}
