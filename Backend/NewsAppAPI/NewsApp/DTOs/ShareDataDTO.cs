using System.ComponentModel.DataAnnotations;

namespace NewsApp.DTOs
{
    public class ShareDataDTO
    {
        public string Platform { get; set; }
        public int UserID { get; set; }

        public int ArticleID { get; set; }
    }

    public class ShareDataReturnDTO
    {
        public int Id { get; set; }
        public string Platform { get; set; }
        public int UserID { get; set; }

        public int ArticleID { get; set; }
    }
}
