using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uber.Migrations
{
    /// <inheritdoc />
    public partial class addingSourceandDestinationPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "destination",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "source",
                table: "trips");

            migrationBuilder.AddColumn<double>(
                name: "DestinationLatitude",
                table: "trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DestinationLongitude",
                table: "trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SourceLatitude",
                table: "trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SourceLongitude",
                table: "trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationLatitude",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "DestinationLongitude",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "SourceLatitude",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "SourceLongitude",
                table: "trips");

            migrationBuilder.AddColumn<string>(
                name: "destination",
                table: "trips",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "source",
                table: "trips",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
