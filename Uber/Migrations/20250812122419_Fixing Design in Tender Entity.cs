using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uber.Migrations
{
    /// <inheritdoc />
    public partial class FixingDesigninTenderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tenders",
                table: "tenders");

            migrationBuilder.AddColumn<Guid>(
                name: "TenderId",
                table: "tenders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenders",
                table: "tenders",
                column: "TenderId");

            migrationBuilder.CreateIndex(
                name: "IX_tenders_TripId",
                table: "tenders",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tenders",
                table: "tenders");

            migrationBuilder.DropIndex(
                name: "IX_tenders_TripId",
                table: "tenders");

            migrationBuilder.DropColumn(
                name: "TenderId",
                table: "tenders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenders",
                table: "tenders",
                columns: new[] { "TripId", "DriverId" });
        }
    }
}
