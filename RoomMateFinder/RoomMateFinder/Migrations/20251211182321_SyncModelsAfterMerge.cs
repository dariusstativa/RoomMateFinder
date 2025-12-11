using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomMateFinder.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelsAfterMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Profiles_ProfileId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_RoomListings_RoomListingId",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Profiles_ProfileId",
                table: "Reviews",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_RoomListings_RoomListingId",
                table: "Reviews",
                column: "RoomListingId",
                principalTable: "RoomListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
    }
}
