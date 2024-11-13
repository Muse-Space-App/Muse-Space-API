using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAcceptingCommissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptingCommissions",
                table: "UserProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptingCommissions",
                table: "UserProfiles");
        }
    }
}
