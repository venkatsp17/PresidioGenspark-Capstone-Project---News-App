namespace NewsApp.Models
{
    public class Enum
    {
        public enum UserType
        {
            Reader,
            Admin
        }

        public enum ArticleStatus
        {
            Pending,
            Edited,
            Approved, 
            Rejected
        }

        public enum Preference
        {
            Like,
            DisLike
        }
    }
}
