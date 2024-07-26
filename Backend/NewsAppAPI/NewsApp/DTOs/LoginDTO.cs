using System.ComponentModel.DataAnnotations;
using static NewsApp.Models.Enum;

namespace NewsApp.DTOs
{
    public class LoginGetDTO
    {
        public string oAuthToken { get; set; }

        public string email { get; set; }

    }

    public class CreatePasswordDTO
    {
        public string userID { get; set; }

        public string Password { get; set; }

    }

    public class LoginGetDTO1
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(4, ErrorMessage = "Password must be at least 4 characters long.")]
        public string Password { get; set; }
    }

    public class LoginReturnDTO
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public UserType Role { get; set; }

    }
}
