using System.ComponentModel.DataAnnotations;


namespace NewsApp.DTOs
{
    public class RegisterGetDTO
    {

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
        public string Name { get; set; }
    }

    public class RegisterReturnDTO
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

    }
}
