using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static NewsApp.Models.Enum;

namespace NewsApp.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        public string OAuthID { get; set; }
        public string OAuthToken { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public byte[] Password { get; set; }
        [Required]
        public byte[] Password_Hashkey { get; set; }
        [Required]
        public UserType Role { get; set; } // Reader or Admin

        // Navigation properties
        public ICollection<Comment> Comments { get; set; }
        public ICollection<SavedArticle> SavedArticles { get; set; }
    }
}
