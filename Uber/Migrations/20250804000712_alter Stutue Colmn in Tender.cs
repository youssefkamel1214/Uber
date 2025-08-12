using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uber.Migrations
{
    /// <inheritdoc />
    public partial class alterStutueColmninTender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "tenders");

            migrationBuilder.AddColumn<string>(
                name: "staute",
                table: "tenders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "staute",
                table: "tenders");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "tenders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
