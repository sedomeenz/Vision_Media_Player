using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision_Media_Player.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaylistMediaRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Playlists_PlaylistId",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Medias_PlaylistId",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "Medias");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Playlists",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Playlists",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PlaylistMedia",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistMedia", x => new { x.PlaylistId, x.MediaId });
                    table.ForeignKey(
                        name: "FK_PlaylistMedia_Medias_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Medias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistMedia_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistMedia_MediaId",
                table: "PlaylistMedia",
                column: "MediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_Users_UserId",
                table: "Playlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_Users_UserId",
                table: "Playlists");

            migrationBuilder.DropTable(
                name: "PlaylistMedia");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Playlists");

            migrationBuilder.AddColumn<int>(
                name: "PlaylistId",
                table: "Medias",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medias_PlaylistId",
                table: "Medias",
                column: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Playlists_PlaylistId",
                table: "Medias",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id");
        }
    }
}
