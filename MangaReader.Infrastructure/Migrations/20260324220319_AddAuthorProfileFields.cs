using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelegramUrl",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TikTokUrl",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "About",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TelegramUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TikTokUrl",
                table: "Users");
        }
    }
}
