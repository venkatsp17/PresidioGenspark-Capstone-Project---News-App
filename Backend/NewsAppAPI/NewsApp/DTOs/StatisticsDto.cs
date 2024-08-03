namespace NewsApp.DTOs
{
    public class StatisticsDto
    {
        public int TotalUserCount { get; set; }
        public int TotalApprovedArticleCount { get; set; }
        public int TotalEditedArticleCount { get; set; }
        public int TotalRejectedArticleCount { get; set; }
        public int TotalPendingArticleCount { get; set; }
        public ArticleDto MostCommentedArticle { get; set; }
        public ArticleDto MostSavedArticle { get; set; }
        public ArticleDto MostSharedArticle { get; set; }
        public IEnumerable<CategoryPreferenceDto> CategoryPreferences { get; set; }
    }

    public class CategoryPreferenceDto
    {
        public string CategoryName { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
