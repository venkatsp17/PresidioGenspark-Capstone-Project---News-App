using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsApp.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }
        [Required]
        public int ArticleID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Article Article
        {
            get; set;
        }
    }
}
