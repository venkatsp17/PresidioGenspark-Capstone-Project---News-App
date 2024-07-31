using NewsApp.DTOs;
using NewsApp.Models;

namespace NewsApp.Mappers
{
    public class ArticleMapper
    {
        public static AdminArticleReturnDTO MapAdminArticleReturnDTO(Article article)
        {

            return new AdminArticleReturnDTO
            {
                ArticleID = article.ArticleID,
                Title = article.Title,
                Content = article.Content,
                Summary = article.Summary,
                AddedAt = article.AddedAt,
                OriginURL = article.OriginURL,
                ImgURL = article.ImgURL,
                CreatedAt = article.CreatedAt,
                ImpScore = article.ImpScore,
                Status = article.Status,
                SaveCount = article.SaveCount,
                CommentCount = article.CommentCount,
                ShareCount = article.ShareCount,
            };
        }
    }
}
