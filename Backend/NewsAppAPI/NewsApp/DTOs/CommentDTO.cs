using System.ComponentModel.DataAnnotations;

namespace NewsApp.DTOs
{
    public class CommentDTO
    {
        public int ArticleID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
    }


    public class CommentReturnDTO
    {
        public int CommentID { get; set; }
        public int ArticleID { get; set; }
        public string Content { get; set; }

        public DateTime Timestamp { get; set; }

        public string UserName { get; set; }
    }
}
