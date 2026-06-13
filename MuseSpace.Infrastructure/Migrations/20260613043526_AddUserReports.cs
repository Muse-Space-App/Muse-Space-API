using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ArtworkId",
                table: "Reports",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "TargetUserId",
                table: "Reports",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_TargetUserId",
                table: "Reports",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_TargetUserId",
                table: "Reports",
                column: "TargetUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_TargetUserId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_TargetUserId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "TargetUserId",
                table: "Reports");

            migrationBuilder.AlterColumn<int>(
                name: "ArtworkId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
