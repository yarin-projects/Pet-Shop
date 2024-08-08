using System.ComponentModel.DataAnnotations;

namespace PetShop.Models.ViewModels
{
    public class CategoryCreateVM
    {
        [Required(ErrorMessage = "Category name is required.")]
        [RegularExpression(@"^\p{L}+$", ErrorMessage = "Name can only contain letters.")]
        [MaxLength(25, ErrorMessage = "Category name cannot be more than 25 characters.")]
        [MinLength(3, ErrorMessage = "Category name has to be atleast 3 characters.")]
        public string Name { get; set; }
    }
}
