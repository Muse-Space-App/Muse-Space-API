using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artwork_Users_CreatorId",
                table: "Artwork");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtworkTags_Artwork_ArtworkId",
                table: "ArtworkTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Artwork_ArtworkId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Artwork_ArtworkId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Artwork_ArtworkId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Artwork_RelatedArtworkId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Artwork_ArtworkId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_Artwork_ArtworkId",
                table: "Shares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Artwork",
                table: "Artwork");

            migrationBuilder.RenameTable(
                name: "Artwork",
                newName: "artwork");

            migrationBuilder.RenameIndex(
                name: "IX_Artwork_IsSoftDeleted",
                table: "artwork",
                newName: "IX_artwork_IsSoftDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Artwork_IsApproved",
                table: "artwork",
                newName: "IX_artwork_IsApproved");

            migrationBuilder.RenameIndex(
                name: "IX_Artwork_CreatorId",
                table: "artwork",
                newName: "IX_artwork_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Artwork_CreatedAtUtc",
                table: "artwork",
                newName: "IX_artwork_CreatedAtUtc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_artwork",
                table: "artwork",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_artwork_Users_CreatorId",
                table: "artwork",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtworkTags_artwork_ArtworkId",
                table: "ArtworkTags",
                column: "ArtworkId",
                principalTable: "artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_artwork_ArtworkId",
                table: "Bookmarks",
                column: "ArtworkId",
                principalTable: "artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_artwork_ArtworkId",
                table: "Comments",
                column: "ArtworkId",
                principalTable: "artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_artwork_ArtworkId",
                table: "Likes",
                column: "ArtworkId",
                principalTable: "artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_artwork_RelatedArtworkId",
                table: "Notifications",
                column: "RelatedArtworkId",
                principalTable: "artwork",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_artwork_ArtworkId",
                table: "Reports",
                column: "ArtworkId",
                principalTable: "artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_artwork_ArtworkId",
                table: "Shares",
                column: "ArtworkId",
                principalTable: "artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_artwork_Users_CreatorId",
                table: "artwork");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtworkTags_artwork_ArtworkId",
                table: "ArtworkTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_artwork_ArtworkId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_artwork_ArtworkId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_artwork_ArtworkId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_artwork_RelatedArtworkId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_artwork_ArtworkId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_artwork_ArtworkId",
                table: "Shares");

            migrationBuilder.DropPrimaryKey(
                name: "PK_artwork",
                table: "artwork");

            migrationBuilder.RenameTable(
                name: "artwork",
                newName: "Artwork");

            migrationBuilder.RenameIndex(
                name: "IX_artwork_IsSoftDeleted",
                table: "Artwork",
                newName: "IX_Artwork_IsSoftDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_artwork_IsApproved",
                table: "Artwork",
                newName: "IX_Artwork_IsApproved");

            migrationBuilder.RenameIndex(
                name: "IX_artwork_CreatorId",
                table: "Artwork",
                newName: "IX_Artwork_CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_artwork_CreatedAtUtc",
                table: "Artwork",
                newName: "IX_Artwork_CreatedAtUtc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Artwork",
                table: "Artwork",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Artwork_Users_CreatorId",
                table: "Artwork",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtworkTags_Artwork_ArtworkId",
                table: "ArtworkTags",
                column: "ArtworkId",
                principalTable: "Artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Artwork_ArtworkId",
                table: "Bookmarks",
                column: "ArtworkId",
                principalTable: "Artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Artwork_ArtworkId",
                table: "Comments",
                column: "ArtworkId",
                principalTable: "Artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Artwork_ArtworkId",
                table: "Likes",
                column: "ArtworkId",
                principalTable: "Artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Artwork_RelatedArtworkId",
                table: "Notifications",
                column: "RelatedArtworkId",
                principalTable: "Artwork",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Artwork_ArtworkId",
                table: "Reports",
                column: "ArtworkId",
                principalTable: "Artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_Artwork_ArtworkId",
                table: "Shares",
                column: "ArtworkId",
                principalTable: "Artwork",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
