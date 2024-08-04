using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsApp.Migrations
{
    public partial class seedcategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "Description", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "Israel-Hamas_War", "Israel-Hamas War", "CUSTOM_CATEGORY" },
                    { 2, "FINANCE", "Finance", "CUSTOM_CATEGORY" },
                    { 3, "Russia-Ukraine_Conflict", "Russia-Ukraine Conflict", "CUSTOM_CATEGORY" },
                    { 4, "EURO_2024", "EURO 2024", "CUSTOM_CATEGORY" },
                    { 5, "EXPLAINERS", "Explainers", "CUSTOM_CATEGORY" },
                    { 6, "LOK_SABHA_ELECTIONS", "Lok Sabha Elections", "CUSTOM_CATEGORY" },
                    { 7, "T20_WORLD_CUP_2024", "T20 WORLD CUP 2024", "CUSTOM_CATEGORY" },
                    { 8, "UNION_BUDGET", "Union Budget", "CUSTOM_CATEGORY" },
                    { 9, "OLYMPICS_2024", "Olympics 2024", "CUSTOM_CATEGORY" },
                    { 10, "national", "India", "NEWS_CATEGORY" },
                    { 11, "business", "Business", "NEWS_CATEGORY" },
                    { 12, "politics", "Politics", "NEWS_CATEGORY" },
                    { 13, "sports", "Sports", "NEWS_CATEGORY" },
                    { 14, "technology", "Technology", "NEWS_CATEGORY" },
                    { 15, "startup", "Startups", "NEWS_CATEGORY" },
                    { 16, "entertainment", "Entertainment", "NEWS_CATEGORY" },
                    { 17, "hatke", "Hatke", "NEWS_CATEGORY" },
                    { 18, "world", "International", "NEWS_CATEGORY" },
                    { 19, "automobile", "Automobile", "NEWS_CATEGORY" },
                    { 20, "science", "Science", "NEWS_CATEGORY" },
                    { 21, "travel", "Travel", "NEWS_CATEGORY" },
                    { 22, "miscellaneous", "Miscellaneous", "NEWS_CATEGORY" },
                    { 23, "fashion", "Fashion", "NEWS_CATEGORY" },
                    { 24, "education", "Education", "NEWS_CATEGORY" },
                    { 25, "Health___Fitness", "Health & Fitness", "NEWS_CATEGORY" },
                    { 26, "Lifestyle", "Lifestyle", "NEWS_CATEGORY" },
                    { 27, "cricket", "Cricket", "CUSTOM_CATEGORY" },
                    { 28, "topstories", "Top Stories", "CUSTOM_CATEGORY" },
                    { 29, "city", "City", "CUSTOM_CATEGORY" },
                    { 80, "experiment", "Experiment", "CUSTOM_CATEGORY" },
                    { 81, "crime", "Crime", "CUSTOM_CATEGORY" },
                    { 82, "Feel_Good_Stories", "Feel_good_stories", "CUSTOM_CATEGORY" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 82);
        }
    }
}
