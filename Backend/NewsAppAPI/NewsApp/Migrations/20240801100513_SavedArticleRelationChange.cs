using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsApp.Migrations
{
    public partial class SavedArticleRelationChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SavedArticles_ArticleID",
                table: "SavedArticles");

            migrationBuilder.CreateIndex(
                name: "IX_SavedArticles_ArticleID",
                table: "SavedArticles",
                column: "ArticleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SavedArticles_ArticleID",
                table: "SavedArticles");

            migrationBuilder.CreateIndex(
                name: "IX_SavedArticles_ArticleID",
                table: "SavedArticles",
                column: "ArticleID",
                unique: true);
        }
    }
}
