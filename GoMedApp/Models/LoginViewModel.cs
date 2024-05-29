using System.ComponentModel.DataAnnotations;

namespace GoMedApp.Models
{
  public class LoginViewModel
{
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    public string? ErrorMessage { get; set; }
    
}
}