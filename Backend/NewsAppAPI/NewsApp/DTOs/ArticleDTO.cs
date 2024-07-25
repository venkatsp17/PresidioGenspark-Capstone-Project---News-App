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
        public int ShareCount { get; set; }

    }
}
