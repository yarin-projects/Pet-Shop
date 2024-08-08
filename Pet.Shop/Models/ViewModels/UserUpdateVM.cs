using System.ComponentModel.DataAnnotations;

namespace PetShop.Models.ViewModels
{
    public class UserUpdateVM
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{3,15}$", ErrorMessage = "Username must be between 3 and 15 characters and contain only English letters and digits.")]
        public string? Username { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9!@]{3,25}$", ErrorMessage = "Password must be between 3 and 25 characters and contain only English letters, digits, and the special characters !@.")]
        public string? OldPassword { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9!@]{3,25}$", ErrorMessage = "Password must be between 3 and 25 characters and contain only English letters, digits, and the special characters !@.")]
        public string? NewPassword { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9!@]{3,25}$", ErrorMessage = "Password must be between 3 and 25 characters and contain only English letters, digits, and the special characters !@.")]
        public string? ConfirmNewPassword { get; set; }
    }
}
