using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;
    }

}
