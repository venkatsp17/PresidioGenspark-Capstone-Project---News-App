using NewsApp.Models;
using static NewsApp.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsApp.DTOs
{
    public class AdminArticleReturnDTO
    {

        public int ArticleID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Summary { get; set; }
        public DateTime AddedAt { get; set; }
        public string OriginURL { get; set; }
        public string ImgURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal ImpScore { get; set; }
        public ArticleStatus Status { get; set; }
        public int SaveCount { get; set; }

        public bool isSaved { get; set; }

        public int CommentCount { get; set; }

        public int ShareCount { get; set; }

    }

    public class AdminArticleEditGetDTO
    {

        [Required(ErrorMessage = "ArticleID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ArticleID must be a positive integer.")]
        public int ArticleID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MinLength(2, ErrorMessage = "Title must be at least 2 characters long.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters long.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Summary is required.")]
        [MinLength(10, ErrorMessage = "Summary must be at least 10 characters long.")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "AddedAt is required.")]
        public DateTime AddedAt { get; set; }

        [Url(ErrorMessage = "OriginURL must be a valid URL.")]
        public string OriginURL { get; set; }

        [Url(ErrorMessage = "ImgURL must be a valid URL.")]
        public string ImgURL { get; set; }

        [Required(ErrorMessage = "CreatedAt is required.")]
        public DateTime CreatedAt { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "ImpScore must be a valid decimal number.")]
        public decimal ImpScore { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public ArticleStatus Status { get; set; }

        [Required(ErrorMessage = "SaveCount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "SaveCount must be a non-negative integer.")]
        public int SaveCount { get; set; }

        [Required(ErrorMessage = "isSaved is required.")]
        public bool isSaved { get; set; }

        [Required(ErrorMessage = "CommentCount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "CommentCount must be a non-negative integer.")]
        public int CommentCount { get; set; }

        [Required(ErrorMessage = "ShareCount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "ShareCount must be a non-negative integer.")]
        public int ShareCount { get; set; }

        [Required(ErrorMessage = "Categories are required.")]
        [MinLength(1, ErrorMessage = "There must be at least one category.")]
        public List<int> Categories { get; set; }

    }


    public class AdminArticlePaginatedReturnDTO
    {

        public IEnumerable<AdminArticleReturnDTO> Articles { get; set; }

        public int totalpages { get; set; }

    }

    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> Categories { get; set; }
        public int CommentCount { get; set; }
        public int SaveCount { get; set; }
        public int ShareCount { get; set; }
    }

}
