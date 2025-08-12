using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uber.Migrations
{
    /// <inheritdoc />
    public partial class FixingsomedesiginerrorsofTenderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "tenders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_tenders_DriverId",
                table: "tenders",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_tenders_drivers_DriverId",
                table: "tenders",
                column: "DriverId",
                principalTable: "drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenders_trips_TripId",
                table: "tenders",
                column: "TripId",
                principalTable: "trips",
                principalColumn: "TripId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenders_drivers_DriverId",
                table: "tenders");

            migrationBuilder.DropForeignKey(
                name: "FK_tenders_trips_TripId",
                table: "tenders");

            migrationBuilder.DropIndex(
                name: "IX_tenders_DriverId",
                table: "tenders");

            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "tenders");
        }
    }
}
