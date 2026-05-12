using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalLanguageToManga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var englishId = new Guid("11111111-1111-1111-1111-111111111111");

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalLanguageId",
                table: "Mangas",
                type: "uuid",
                nullable: false,
                defaultValue: englishId);

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_OriginalLanguageId",
                table: "Mangas",
                column: "OriginalLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mangas_Languages_OriginalLanguageId",
                table: "Mangas",
                column: "OriginalLanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mangas_Languages_OriginalLanguageId",
                table: "Mangas");

            migrationBuilder.DropIndex(
                name: "IX_Mangas_OriginalLanguageId",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "OriginalLanguageId",
                table: "Mangas");
        }
    }
}
