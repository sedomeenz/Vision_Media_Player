using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vision_Media_Player.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMediaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_Users_UserId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Playlists");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Medias",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Medias",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Medias",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Medias",
                newName: "Title");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Playlists",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_Users_UserId",
                table: "Playlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
