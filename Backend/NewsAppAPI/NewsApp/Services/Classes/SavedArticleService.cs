using Microsoft.AspNetCore.SignalR;
using NewsApp.DTOs;
using NewsApp.Exceptions;
using NewsApp.Mappers;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace NewsApp.Services.Classes
{
    public class SavedArticleService : ISavedArticleService
    {

        private readonly ISavedRepository _savedArticleRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IHubContext<CommentHub> _hubContext;
        private readonly ILogger<SavedArticleService> _logger;

        public SavedArticleService(IHubContext<CommentHub> hubContext, ISavedRepository savedArticleRepository, IArticleRepository articleRepository, ILogger<SavedArticleService> logger)
        {
            _savedArticleRepository = savedArticleRepository;
            _articleRepository = articleRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<bool> CheckForSaved(int articleid, int userid)
        {
            try
            {
                var article = await _savedArticleRepository.GetBy2Id("ArticleID", articleid.ToString(), "UserID", userid.ToString());
                return await Task.FromResult(article != null);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging1
                _logger.LogError($"Error in CheckForSaved: {ex.Message}", ex);
                return await Task.FromResult(false);
            }
        }
        [ExcludeFromCodeCoverage]
        public async Task<int> SaveAndUnSaveArticle(int articleid, int userid)
        {
            var article = await _savedArticleRepository.GetBy2Id("ArticleID",articleid.ToString(),"UserID",userid.ToString());

            var savecountArticle = await _articleRepository.Get("ArticleID", articleid.ToString());
            if (article == null) {
                var newSaved = new SavedArticle(){
                    ArticleID = articleid,
                    UserID = userid,
                    SavedAt = DateTime.Now,
                };
                await _savedArticleRepository.Add(newSaved);
                if(newSaved == null) { 
                    throw new UnableToAddItemException();
                }               
                savecountArticle.SaveCount += 1;
                await _articleRepository.Update(savecountArticle, savecountArticle.ArticleID.ToString());
                await _hubContext.Clients.Group(articleid.ToString()).SendAsync("UpdateSaveArticleCount", articleid.ToString(), savecountArticle.SaveCount);
                return newSaved.SavedArticleID;
            }
            var deleted = await _savedArticleRepository.Delete(article.SavedArticleID.ToString());
            if(deleted == null)
            {
                throw new UnableToUpdateItemException();
            }
            if(savecountArticle.SaveCount > 0) { savecountArticle.SaveCount -= 1; } else { savecountArticle.SaveCount = 0; }
            
            await _articleRepository.Update(savecountArticle, savecountArticle.ArticleID.ToString());
            await _hubContext.Clients.Group(articleid.ToString()).SendAsync("UpdateSaveArticleCount", articleid.ToString(), savecountArticle.SaveCount);
            return deleted.SavedArticleID;

        }

        public async Task<AdminArticlePaginatedReturnDTO> GetAllSavedArticles(int userid, int pageNumber, int pageSize, string query)
        {
            var allArticles = await _savedArticleRepository.GetAll("UserID", userid.ToString());

            if (allArticles == null || allArticles.Count() == 0)
            {
                throw new NoAvailableItemException();
            }

            var articlesList = new List<AdminArticleReturnDTO>();

            foreach (var article in allArticles)
            {
                var result = await _articleRepository.Get("ArticleID", article.ArticleID.ToString()); 
                AdminArticleReturnDTO adminArticleReturnDTO = ArticleMapper.MapAdminArticleReturnDTO(result);
                if (userid != 0)
                {
                    adminArticleReturnDTO.isSaved = true;
                }
                articlesList.Add(adminArticleReturnDTO);
            }

            if (query != "null")
            {
                articlesList = articlesList
                    .Where(article => article.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                      article.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }


            int skip = (pageNumber - 1) * pageSize;

            var paginatedArticles = articlesList
               .OrderByDescending(article => article.CreatedAt)
               .Skip(skip)
               .Take(pageSize);

            var totalPages = (int)Math.Ceiling(allArticles.Count() / (double)pageSize);

           

            return new AdminArticlePaginatedReturnDTO
            {
                Articles = articlesList,
                totalpages = totalPages
            };

        }

    }
}
