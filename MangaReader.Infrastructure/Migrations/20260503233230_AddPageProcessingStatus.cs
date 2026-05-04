using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPageProcessingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OcrProcessedAt",
                table: "Pages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingError",
                table: "Pages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingStatus",
                table: "Pages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TranslationProcessedAt",
                table: "Pages",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OcrProcessedAt",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ProcessingError",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "TranslationProcessedAt",
                table: "Pages");
        }
    }
}
