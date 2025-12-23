using System.ComponentModel.DataAnnotations;

namespace HelpdeskModel.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [Display(Name = "Login as")]
        public string Role { get; set; } 
    }
}
