using NewsApp.Exceptions;
using NewsApp.Models;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Interfaces;
using Newtonsoft.Json.Linq;
using Sprache;
using System.Security.Policy;
using System;
using static NewsApp.Models.Enum;
using NewsApp.DTOs;
using NewsApp.Repositories.Classes;
using NewsApp.Mappers;

namespace NewsApp.Services.Classes
{
    public class ArticleService : IArticleService
    {
        private readonly IRepository<string, Article, string> _articleRepository;
        private readonly IRepository<string, Category, string> _categoryRepository;
        private readonly IArticleCategoryRepository _articlecategoryRepository;
        private readonly HttpClient _httpClient;

        public ArticleService(IRepository<string, Article, string> articleRepository, HttpClient httpClient, IRepository<string, Category, string> categoryRepository, IArticleCategoryRepository articlecategoryRepository)
        {
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _httpClient = httpClient;
            _articlecategoryRepository = articlecategoryRepository;
        }

        public async Task<AdminArticleReturnDTO> ChangeArticleStatus(string articleId, ArticleStatus articleStatus)
        {
            var existingArticle = await _articleRepository.Get("ArticleID", articleId);
            if(existingArticle == null)
            {
                throw new ItemNotFoundException();
            }
            // Update existing article
            existingArticle.Status = articleStatus;

            var result = await _articleRepository.Update(existingArticle, existingArticle.ArticleID.ToString());
            if(result == null)
            {
                throw new UnableToUpdateItemException();
            }
            return ArticleMapper.MapAdminArticleReturnDTO(result);             
        }

        public async Task<IEnumerable<AdminArticleReturnDTO>> GetTopStoryArticlesAsync(int pageNumber = 1, int pageSize = 10)
        {

            var response = await _httpClient.GetStringAsync("https://m.inshorts.com/api/in/en/news?category=top_stories&max_limit=5&include_card_data=true");

            // Fetch articles from the third-party API
            var json = JObject.Parse(response);
            var newsList = json["data"]?["news_list"]?.ToArray();


            if (newsList == null || newsList.Length == 0)
            {
                throw new ItemNotFoundException();
            }

            foreach (var newsItem in newsList)
            {
                var newsObj = newsItem["news_obj"];
                var version = (int)newsItem["version"];

                var hashId = (string)newsObj["hash_id"];
                var oldHashId = (string)newsObj["old_hash_id"];

                var categories = newsObj["category_names"];

                var article = new Article
                {
                    OldHashID = oldHashId,
                    HashID = hashId,
                    Title = (string)newsObj["title"],
                    Content = (string)newsObj["content"],
                    Summary = (string)newsObj["bottom_headline"],
                    AddedAt = DateTime.UtcNow,
                    OriginURL = (string)newsObj["source_url"],
                    ImgURL = (string)newsObj["image_url"],
                    CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds((long)newsObj["created_at"]).UtcDateTime,
                    ImpScore = (decimal)newsObj["impressive_score"],
                    ShareCount = 0,
                    Status = ArticleStatus.Pending
                };

                // Determine the identifier to use for lookup
                var existingArticle = version == 0
                ? await _articleRepository.Get("HashID", hashId)
                : await _articleRepository.Get("OldHashID", oldHashId);

                if(existingArticle == null)
                {
                    var result = await _articleRepository.Add(article);
                    if (result.ArticleID == null)
                    {
                        throw new UnableToAddItemException();
                    }
                    foreach (var category in categories)
                    {
                        string category1 = (string)category;
                        int CategoryID;

                        var validcategory = await _categoryRepository.Get("Description", category1);
                        if(validcategory == null)
                        {
                            Category newcategory = new Category()
                            {
                                Description = category1,
                                Name = char.ToUpper(category1[0]) + category1.Substring(1).ToLower()

                            };
                            var result1 = await _categoryRepository.Add(newcategory);
                            if (result1.CategoryID == null)
                            {
                                throw new UnableToAddItemException();
                            }
                            CategoryID = result1.CategoryID;
                        }
                        else
                        {
                            CategoryID = validcategory.CategoryID;
                        }

                        ArticleCategory articleCategory = new ArticleCategory()
                        {
                            ArticleID = result.ArticleID,
                            CategoryID = CategoryID,
                        };

                        var articlecategory = await _articlecategoryRepository.Add(articleCategory);
                        if (articlecategory.ArticleID == null)
                        {
                            throw new UnableToAddItemException();
                        }
                    }
                }
                else
                {
                    // Update existing article
                    existingArticle.Title = article.Title;
                    existingArticle.Content = article.Content;
                    existingArticle.Summary = article.Summary;
                    existingArticle.ImgURL = article.ImgURL;
                    existingArticle.AddedAt = article.AddedAt;
                    existingArticle.OriginURL = article.OriginURL;
                    existingArticle.CreatedAt = article.CreatedAt;
                    existingArticle.ImpScore = article.ImpScore;

                    // Update both HashID and OldHashID
                    existingArticle.HashID = hashId;
                    existingArticle.OldHashID = oldHashId;

                    await _articleRepository.Update(existingArticle, existingArticle.ArticleID.ToString());
                }
      
            }

            // Return paginated articles sorted by CreatedAt descending
            return await GetPaginatedArticlesAsync(pageNumber, pageSize);
        }

        private async Task<IEnumerable<AdminArticleReturnDTO>> GetPaginatedArticlesAsync(int pageNumber, int pageSize)
        {
            // Fetch all articles
            var allArticles = await _articleRepository.GetAll("","");

            if (allArticles == null || allArticles.Count() == 0)
            {
                throw new NoAvailableItemException();
            }

            // Calculate the skip and take parameters for pagination
            int skip = (pageNumber - 1) * pageSize;

            var paginatedArticles = allArticles
               .OrderByDescending(article => article.CreatedAt)
               .Skip(skip)
               .Take(pageSize);

            return paginatedArticles.Select(x => ArticleMapper.MapAdminArticleReturnDTO(x));
        }



    }
}
