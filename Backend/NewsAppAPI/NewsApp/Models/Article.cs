using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static NewsApp.Models.Enum;

namespace NewsApp.Models
{
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string Summary { get; set; }
        [Required]
        public DateTime AddedAt { get; set; }
        [Required]
        public string OriginURL { get; set; }
        [Required]
        public string ImgURL { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public decimal ImpScore { get; set; }
        [Required]
        public string OldHashID { get; set; }
        [Required]
        public string HashID { get; set; }
        [Required]
        public ArticleStatus Status { get; set; }
        public int SaveCount { get; set; }

        // Navigation properties
        public ICollection<Comment> Comments { get; set; }
        public ICollection<ArticleCategory> ArticleCategories { get; set; }
    }
}
