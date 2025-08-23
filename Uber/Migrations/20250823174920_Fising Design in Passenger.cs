using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uber.Migrations
{
    /// <inheritdoc />
    public partial class FisingDesigninPassenger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationCount",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<double>(
                name: "HomeLatitude",
                table: "AspNetUsers",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HomeLongitude",
                table: "AspNetUsers",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WorkLatitude",
                table: "AspNetUsers",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WorkLongitude",
                table: "AspNetUsers",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeLatitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HomeLongitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WorkLatitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WorkLongitude",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CancellationCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }
    }
}
