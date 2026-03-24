using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addauthorizationforauthordashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MangaCover_Mangas_MangaId",
                table: "MangaCover");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Languages_PreferredLanguageId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MangaCover",
                table: "MangaCover");

            migrationBuilder.RenameTable(
                name: "MangaCover",
                newName: "MangaCovers");

            migrationBuilder.RenameIndex(
                name: "IX_MangaCover_MangaId",
                table: "MangaCovers",
                newName: "IX_MangaCovers_MangaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MangaCovers",
                table: "MangaCovers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_AuthorId",
                table: "Mangas",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_MangaCovers_Mangas_MangaId",
                table: "MangaCovers",
                column: "MangaId",
                principalTable: "Mangas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mangas_Users_AuthorId",
                table: "Mangas",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Languages_PreferredLanguageId",
                table: "Users",
                column: "PreferredLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MangaCovers_Mangas_MangaId",
                table: "MangaCovers");

            migrationBuilder.DropForeignKey(
                name: "FK_Mangas_Users_AuthorId",
                table: "Mangas");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Languages_PreferredLanguageId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Mangas_AuthorId",
                table: "Mangas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MangaCovers",
                table: "MangaCovers");

            migrationBuilder.RenameTable(
                name: "MangaCovers",
                newName: "MangaCover");

            migrationBuilder.RenameIndex(
                name: "IX_MangaCovers_MangaId",
                table: "MangaCover",
                newName: "IX_MangaCover_MangaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MangaCover",
                table: "MangaCover",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MangaCover_Mangas_MangaId",
                table: "MangaCover",
                column: "MangaId",
                principalTable: "Mangas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Languages_PreferredLanguageId",
                table: "Users",
                column: "PreferredLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
