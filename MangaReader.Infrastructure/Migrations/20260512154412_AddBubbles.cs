using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBubbles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BubbleId",
                table: "Phrases",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Bubble",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<decimal>(type: "numeric", nullable: false),
                    Y = table.Column<decimal>(type: "numeric", nullable: false),
                    Width = table.Column<decimal>(type: "numeric", nullable: false),
                    Height = table.Column<decimal>(type: "numeric", nullable: false),
                    OriginalText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bubble", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bubble_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BubbleTranslation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BubbleId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BubbleTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BubbleTranslation_Bubble_BubbleId",
                        column: x => x.BubbleId,
                        principalTable: "Bubble",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BubbleTranslation_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Phrases_BubbleId",
                table: "Phrases",
                column: "BubbleId");

            migrationBuilder.CreateIndex(
                name: "IX_Bubble_PageId",
                table: "Bubble",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_BubbleTranslation_BubbleId_LanguageId",
                table: "BubbleTranslation",
                columns: new[] { "BubbleId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BubbleTranslation_LanguageId",
                table: "BubbleTranslation",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Phrases_Bubble_BubbleId",
                table: "Phrases",
                column: "BubbleId",
                principalTable: "Bubble",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Phrases_Bubble_BubbleId",
                table: "Phrases");

            migrationBuilder.DropTable(
                name: "BubbleTranslation");

            migrationBuilder.DropTable(
                name: "Bubble");

            migrationBuilder.DropIndex(
                name: "IX_Phrases_BubbleId",
                table: "Phrases");

            migrationBuilder.DropColumn(
                name: "BubbleId",
                table: "Phrases");
        }
    }
}
