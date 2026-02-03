using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTok.Services.News.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "News");

            migrationBuilder.CreateTable(
                name: "NewsArticles",
                schema: "News",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticles", x => x.Uuid);
                });

            migrationBuilder.CreateTable(
                name: "NewsArticleEntities",
                schema: "News",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    Industry = table.Column<string>(type: "text", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticleEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsArticleEntities_NewsArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalSchema: "News",
                        principalTable: "NewsArticles",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticleEntities_ArticleId",
                schema: "News",
                table: "NewsArticleEntities",
                column: "ArticleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsArticleEntities",
                schema: "News");

            migrationBuilder.DropTable(
                name: "NewsArticles",
                schema: "News");
        }
    }
}
