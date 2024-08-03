using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Mappers;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Interfaces;
using static NewsApp.Models.Enum;

namespace NewsApp.Services.Classes
{
    public class RankArticleService : IRankArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ISavedArticleService _savedArticleService;
        private readonly IUserRepository _userRepository;
        private readonly IUserPreferenceRepository _userPreferenceRepository;

        public RankArticleService(IArticleRepository articleRepository, ISavedArticleService savedArticleService, IUserRepository userRepository, IUserPreferenceRepository userPreferenceRepository) { 
            _articleRepository = articleRepository;
            _savedArticleService = savedArticleService;
            _userRepository = userRepository;
            _userPreferenceRepository = userPreferenceRepository;
        }

        public async Task<StatisticsDto> GetAllStatistics()
        {
            var totalUserCount = await _userRepository.GetAllUserCountAsync();

            var totalApprovedArticleCount = await _articleRepository.GetAllArticleCountByStatus(ArticleStatus.Approved);
            var totalEditedArticleCount = await _articleRepository.GetAllArticleCountByStatus(ArticleStatus.Edited);
            var totalRejectedArticleCount = await _articleRepository.GetAllArticleCountByStatus(ArticleStatus.Rejected);
            var totalPendingArticleCount = await _articleRepository.GetAllArticleCountByStatus(ArticleStatus.Pending);

            var mostCommentedArticle = await _articleRepository.MostInteractedArticle("comment");
            var mostSavedArticle = await _articleRepository.MostInteractedArticle("saved");
            var mostSharedArticle = await _articleRepository.MostInteractedArticle("shared");

            var categoryPreferences = await _userPreferenceRepository.LikedDiskedAriclesORder();

            return new StatisticsDto
            {

                TotalUserCount = totalUserCount,
                TotalApprovedArticleCount = totalApprovedArticleCount,
                TotalEditedArticleCount = totalEditedArticleCount,
                TotalRejectedArticleCount = totalRejectedArticleCount,
                TotalPendingArticleCount = totalPendingArticleCount,
                MostCommentedArticle = mostCommentedArticle,
                MostSavedArticle = mostSavedArticle,
                MostSharedArticle = mostSharedArticle,
                CategoryPreferences = categoryPreferences
            };
        }

        //Removed 7 days constraint for now
        public async Task<IEnumerable<AdminArticleReturnDTO>> RankTop3Articles(int category, int userid)
        {
            var articles = await _articleRepository.GetArticlesForRanking(category);

            if(!articles.Any()) {
                throw new NoAvailableItemException();
            }

            var rankedArticles = articles.Select(a => new
            {
                Article = a,
                Score = (a.CommentCount * 1) + (a.SaveCount * 2) + (a.ShareCount * 3)
            })
            .OrderByDescending(a => a.Score)
            .Take(3)
            .ToList();

            var articlesList = new List<AdminArticleReturnDTO>();

            foreach (var article in rankedArticles)
            {
                var result = ArticleMapper.MapAdminArticleReturnDTO(article.Article);

                result.isSaved = await _savedArticleService.CheckForSaved(article.Article.ArticleID, userid);

                articlesList.Add(result);
            }

            return await Task.FromResult(articlesList);
        }
    }
}
