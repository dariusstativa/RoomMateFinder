using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomMateFinder.Migrations
{
    /// <inheritdoc />
    public partial class AddMessagesAndReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "TargetUserId",
                table: "Reviews",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileId",
                table: "Reviews",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoomListingId",
                table: "Reviews",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProfileId",
                table: "Reviews",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RoomListingId",
                table: "Reviews",
                column: "RoomListingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Profiles_ProfileId",
                table: "Reviews",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_RoomListings_RoomListingId",
                table: "Reviews",
                column: "RoomListingId",
                principalTable: "RoomListings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Profiles_ProfileId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_RoomListings_RoomListingId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ProfileId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_RoomListingId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "RoomListingId",
                table: "Reviews");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetUserId",
                table: "Reviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
