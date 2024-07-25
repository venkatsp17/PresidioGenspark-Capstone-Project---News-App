using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsApp.Models
{
    public class ArticleCategory
    {
        [Key, Column(Order = 0)]
        public int ArticleID { get; set; }

        [Key, Column(Order = 1)]
        public int CategoryID { get; set; }

        public Article Article { get; set; }

        public Category Category { get; set; }  
    }
}
