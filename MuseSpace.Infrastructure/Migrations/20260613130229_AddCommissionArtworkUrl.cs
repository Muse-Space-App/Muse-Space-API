using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommissionArtworkUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArtworkUrl",
                table: "Commissions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtworkUrl",
                table: "Commissions");
        }
    }
}
