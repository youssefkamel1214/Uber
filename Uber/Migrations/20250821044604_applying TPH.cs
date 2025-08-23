using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uber.Migrations
{
    /// <inheritdoc />
    public partial class applyingTPH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenders_Drivers_DriverId",
                table: "tenders");

            migrationBuilder.DropForeignKey(
                name: "FK_trips_Drivers_DriverId",
                table: "trips");

            migrationBuilder.DropForeignKey(
                name: "FK_trips_Passengers_PassengerId",
                table: "trips");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropTable(
                name: "UberUsers");

            migrationBuilder.AddColumn<int>(
                name: "CancellationCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicensePlate",
                table: "AspNetUsers",
                type: "character varying(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfReviews",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SSN",
                table: "AspNetUsers",
                type: "character varying(14)",
                maxLength: 14,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "AspNetUsers",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "isAvailable",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "paymentMethod",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "rating",
                table: "AspNetUsers",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_LicensePlate",
                table: "AspNetUsers",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_SSN",
                table: "AspNetUsers",
                column: "SSN",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tenders_AspNetUsers_DriverId",
                table: "tenders",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trips_AspNetUsers_DriverId",
                table: "trips",
                column: "DriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_AspNetUsers_PassengerId",
                table: "trips",
                column: "PassengerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenders_AspNetUsers_DriverId",
                table: "tenders");

            migrationBuilder.DropForeignKey(
                name: "FK_trips_AspNetUsers_DriverId",
                table: "trips");

            migrationBuilder.DropForeignKey(
                name: "FK_trips_AspNetUsers_PassengerId",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_LicensePlate",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_SSN",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CancellationCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LicensePlate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NumberOfReviews",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SSN",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "isAvailable",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "paymentMethod",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "rating",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "UberUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NumberOfReviews = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UberUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UberUsers_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LicensePlate = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    SSN = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    isAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drivers_UberUsers_Id",
                        column: x => x.Id,
                        principalTable: "UberUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CancellationCount = table.Column<int>(type: "integer", nullable: false),
                    paymentMethod = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passengers_UberUsers_Id",
                        column: x => x.Id,
                        principalTable: "UberUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_LicensePlate",
                table: "Drivers",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_SSN",
                table: "Drivers",
                column: "SSN",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tenders_Drivers_DriverId",
                table: "tenders",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trips_Drivers_DriverId",
                table: "trips",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_Passengers_PassengerId",
                table: "trips",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id");
        }
    }
}
