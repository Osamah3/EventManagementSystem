using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trion.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVenueToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "venueid",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Event_venueid",
                table: "Event",
                column: "venueid");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Venue_venueid",
                table: "Event",
                column: "venueid",
                principalTable: "Venue",
                principalColumn: "VenueId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Venue_venueid",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_venueid",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "venueid",
                table: "Event");
        }
    }
}
