using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsApp.Models
{
    public class ShareData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Platform { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int ArticleID { get; set; }
    }
}
