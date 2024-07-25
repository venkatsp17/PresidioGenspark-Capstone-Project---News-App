using static NewsApp.Models.Enum;

namespace NewsApp.DTOs
{
    public class LoginGetDTO
    {
        public string oAuthToken { get; set; }
    }

    public class LoginReturnDTO
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string OAuthToken { get; set; }
        public UserType Role { get; set; }

    }
}
