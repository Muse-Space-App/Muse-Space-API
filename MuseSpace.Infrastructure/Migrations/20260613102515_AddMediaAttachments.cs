using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentType",
                table: "GroupPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "GroupPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentType",
                table: "CommissionMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "CommissionMessages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentType",
                table: "GroupPosts");

            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "GroupPosts");

            migrationBuilder.DropColumn(
                name: "AttachmentType",
                table: "CommissionMessages");

            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "CommissionMessages");
        }
    }
}
