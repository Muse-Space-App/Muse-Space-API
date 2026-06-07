using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiDescription",
                table: "Artwork",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Artwork",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Artwork",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiDescription",
                table: "Artwork");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Artwork");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Artwork");
        }
    }
}
