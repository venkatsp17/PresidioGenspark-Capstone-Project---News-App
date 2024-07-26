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
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace NewsApp.Services.Classes
{
    public class ArticleService : IArticleService
    {
        private readonly IRepository<string, Article, string> _articleRepository;
        private readonly IRepository<string, Category, string> _categoryRepository;
        private readonly IArticleCategoryRepository _articlecategoryRepository;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        private const string MinNewsIdCacheKey = "MinNewsId";

        public ArticleService(IRepository<string, Article, string> articleRepository, 
            HttpClient httpClient, 
            IRepository<string, Category, string> categoryRepository, 
            IArticleCategoryRepository articlecategoryRepository,
             IMemoryCache cache
            )
        {
            _articleRepository = articleRepository;
            _categoryRepository = categoryRepository;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://m.inshorts.com/en/read");
            _articlecategoryRepository = articlecategoryRepository;
            _cache = cache;
        }

        public async Task<AdminArticleReturnDTO> ChangeArticleStatus(string articleId, ArticleStatus articleStatus)
        {
            var existingArticle = await _articleRepository.Get("ArticleID", articleId);
            if(existingArticle == null)
            {
                throw new ItemNotFoundException();
            }
            existingArticle.Status = articleStatus;

            var result = await _articleRepository.Update(existingArticle, existingArticle.ArticleID.ToString());
            if(result == null)
            {
                throw new UnableToUpdateItemException();
            }
            return ArticleMapper.MapAdminArticleReturnDTO(result);             
        }

        public async Task FetchAndSaveArticlesAsync()
        {
            var minNewsId = _cache.Get<string>(MinNewsIdCacheKey);
            string url = minNewsId == null
                ? "https://m.inshorts.com/api/in/en/news?category=top_stories&max_limit=10&include_card_data=true"
                : $"https://m.inshorts.com/api/in/en/news?category=top_stories&max_limit=10&include_card_data=true&news_offset={minNewsId}";

            var response = await _httpClient.GetStringAsync(url);

            var json = JObject.Parse(response);
            var newsList = json["data"]?["news_list"]?.ToArray();
            var newMinNewsId = json["data"]?["min_news_id"]?.ToString();

            if (!string.IsNullOrEmpty(newMinNewsId))
            {
                _cache.Set(MinNewsIdCacheKey, newMinNewsId);
            }

            if (newsList == null || newsList.Length == 0)
            {
                return;
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

                var existingArticle = version == 0
                    ? await _articleRepository.Get("HashID", hashId)
                    : await _articleRepository.Get("OldHashID", oldHashId);

                if (existingArticle == null)
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

                        var validCategory = await _categoryRepository.Get("Description", category1);
                        if (validCategory == null)
                        {
                            Category newCategory = new Category()
                            {
                                Description = category1,
                                Name = char.ToUpper(category1[0]) + category1.Substring(1).ToLower()
                            };
                            var result1 = await _categoryRepository.Add(newCategory);
                            if (result1.CategoryID == null)
                            {
                                throw new UnableToAddItemException();
                            }
                            CategoryID = result1.CategoryID;
                        }
                        else
                        {
                            CategoryID = validCategory.CategoryID;
                        }

                        ArticleCategory articleCategory = new ArticleCategory()
                        {
                            ArticleID = result.ArticleID,
                            CategoryID = CategoryID,
                        };

                        var articleCategoryResult = await _articlecategoryRepository.Add(articleCategory);
                        if (articleCategoryResult.ArticleID == null)
                        {
                            throw new UnableToAddItemException();
                        }
                    }
                    //Add Top Stories category to every article fetched
                    ArticleCategory articleCategory1 = new ArticleCategory()
                    {
                        ArticleID = result.ArticleID,
                        CategoryID = 28,
                    };
                    var articleCategoryResult1 = await _articlecategoryRepository.Add(articleCategory1);
                    if (articleCategoryResult1.ArticleID == null)
                    {
                        throw new UnableToAddItemException();
                    }
                }
                else
                {

                    if(existingArticle.HashID != hashId)
                    {
                        existingArticle.Title = article.Title;
                        existingArticle.Content = article.Content;
                        existingArticle.Summary = article.Summary;
                        existingArticle.ImgURL = article.ImgURL;
                        existingArticle.AddedAt = article.AddedAt;
                        existingArticle.OriginURL = article.OriginURL;
                        existingArticle.CreatedAt = article.CreatedAt;
                        existingArticle.ImpScore = article.ImpScore;

                        existingArticle.HashID = hashId;
                        existingArticle.OldHashID = oldHashId;
                        existingArticle.Status = ArticleStatus.Pending; //Edited article change status to pending

                        await _articleRepository.Update(existingArticle, existingArticle.ArticleID.ToString());
                    }
                }
            }
        }

        public async Task<AdminArticlePaginatedReturnDTO> GetPaginatedArticlesAsync(int pageNumber, int pageSize, string status)
        {

            var allArticles = await _articleRepository.GetAll("Status", status);

            if (allArticles == null || allArticles.Count() == 0)
            {
                throw new NoAvailableItemException();
            }


            int skip = (pageNumber - 1) * pageSize;

            var paginatedArticles = allArticles
               .OrderByDescending(article => article.CreatedAt)
               .Skip(skip)
               .Take(pageSize);

            var totalPages = (int)Math.Ceiling(allArticles.Count() / (double)pageSize);

            return new AdminArticlePaginatedReturnDTO{
                Articles=paginatedArticles.Select(x => ArticleMapper.MapAdminArticleReturnDTO(x)),
                totalpages=totalPages
            };
        }


        public async Task<AdminArticleReturnDTO> EditArticleData(AdminArticleReturnDTO adminArticleReturnDTO)
        {
            var article = await _articleRepository.Get("ArticleID", adminArticleReturnDTO.ArticleID.ToString());

            if (article == null)
            {
                throw new ItemNotFoundException();
            }

            article.Title = adminArticleReturnDTO.Title;
            article.Content = adminArticleReturnDTO.Content;
            article.ImgURL = adminArticleReturnDTO.ImgURL;
            article.Summary = adminArticleReturnDTO.Summary;
            article.OriginURL =adminArticleReturnDTO.OriginURL;
            article.Status = ArticleStatus.Edited;

            var result = await _articleRepository.Update(article, article.ArticleID.ToString());
            if(result != null) { 
                return ArticleMapper.MapAdminArticleReturnDTO(result);
            }

            throw new UnableToUpdateItemException();
        }



    }
}
