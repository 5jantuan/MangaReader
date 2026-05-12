using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBubblesAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bubble_Pages_PageId",
                table: "Bubble");

            migrationBuilder.DropForeignKey(
                name: "FK_BubbleTranslation_Bubble_BubbleId",
                table: "BubbleTranslation");

            migrationBuilder.DropForeignKey(
                name: "FK_BubbleTranslation_Languages_LanguageId",
                table: "BubbleTranslation");

            migrationBuilder.DropForeignKey(
                name: "FK_Phrases_Bubble_BubbleId",
                table: "Phrases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BubbleTranslation",
                table: "BubbleTranslation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bubble",
                table: "Bubble");

            migrationBuilder.RenameTable(
                name: "BubbleTranslation",
                newName: "BubbleTranslations");

            migrationBuilder.RenameTable(
                name: "Bubble",
                newName: "Bubbles");

            migrationBuilder.RenameIndex(
                name: "IX_BubbleTranslation_LanguageId",
                table: "BubbleTranslations",
                newName: "IX_BubbleTranslations_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_BubbleTranslation_BubbleId_LanguageId",
                table: "BubbleTranslations",
                newName: "IX_BubbleTranslations_BubbleId_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_Bubble_PageId",
                table: "Bubbles",
                newName: "IX_Bubbles_PageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BubbleTranslations",
                table: "BubbleTranslations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bubbles",
                table: "Bubbles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bubbles_Pages_PageId",
                table: "Bubbles",
                column: "PageId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BubbleTranslations_Bubbles_BubbleId",
                table: "BubbleTranslations",
                column: "BubbleId",
                principalTable: "Bubbles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BubbleTranslations_Languages_LanguageId",
                table: "BubbleTranslations",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phrases_Bubbles_BubbleId",
                table: "Phrases",
                column: "BubbleId",
                principalTable: "Bubbles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bubbles_Pages_PageId",
                table: "Bubbles");

            migrationBuilder.DropForeignKey(
                name: "FK_BubbleTranslations_Bubbles_BubbleId",
                table: "BubbleTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_BubbleTranslations_Languages_LanguageId",
                table: "BubbleTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_Phrases_Bubbles_BubbleId",
                table: "Phrases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BubbleTranslations",
                table: "BubbleTranslations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bubbles",
                table: "Bubbles");

            migrationBuilder.RenameTable(
                name: "BubbleTranslations",
                newName: "BubbleTranslation");

            migrationBuilder.RenameTable(
                name: "Bubbles",
                newName: "Bubble");

            migrationBuilder.RenameIndex(
                name: "IX_BubbleTranslations_LanguageId",
                table: "BubbleTranslation",
                newName: "IX_BubbleTranslation_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_BubbleTranslations_BubbleId_LanguageId",
                table: "BubbleTranslation",
                newName: "IX_BubbleTranslation_BubbleId_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_Bubbles_PageId",
                table: "Bubble",
                newName: "IX_Bubble_PageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BubbleTranslation",
                table: "BubbleTranslation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bubble",
                table: "Bubble",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bubble_Pages_PageId",
                table: "Bubble",
                column: "PageId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BubbleTranslation_Bubble_BubbleId",
                table: "BubbleTranslation",
                column: "BubbleId",
                principalTable: "Bubble",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BubbleTranslation_Languages_LanguageId",
                table: "BubbleTranslation",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phrases_Bubble_BubbleId",
                table: "Phrases",
                column: "BubbleId",
                principalTable: "Bubble",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
