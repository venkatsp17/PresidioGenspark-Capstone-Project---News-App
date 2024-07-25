using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsApp.Models
{
    public class SavedArticle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SavedArticleID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int ArticleID { get; set; }
        [Required]
        public DateTime SavedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Article Article { get; set; }
    }
}
