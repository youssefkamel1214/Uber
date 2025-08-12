using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uber.Migrations
{
    /// <inheritdoc />
    public partial class fixingDesigninReviewentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "reviews",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "reviews");
        }
    }
}
