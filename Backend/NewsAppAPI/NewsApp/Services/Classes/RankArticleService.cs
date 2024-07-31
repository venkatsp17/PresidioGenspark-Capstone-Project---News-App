using Google;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Mappers;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Interfaces;

namespace NewsApp.Services.Classes
{
    public class RankArticleService : IRankArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ISavedArticleService _savedArticleService;

        public RankArticleService(IArticleRepository articleRepository, ISavedArticleService savedArticleService) { 
            _articleRepository = articleRepository;
            _savedArticleService = savedArticleService;
        }

        //Removed 7 days constraint for now
        public async Task<IEnumerable<AdminArticleReturnDTO>> RankTop3Articles(int category, int userid)
        {
            var articles = await _articleRepository.GetArticlesForRanking(category);

            if(articles == null || articles.Count() == 0) {
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
