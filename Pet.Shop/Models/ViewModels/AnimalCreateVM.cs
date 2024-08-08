using System.ComponentModel.DataAnnotations;

namespace PetShop.Models.ViewModels
{
    public class AnimalCreateVM
    {
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^\p{L}{3,15}$", ErrorMessage = "Name can only contain letters and be between 3-15 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 25, ErrorMessage = "Age must be between 1 and 25.")]
        public int? Age { get; set; }

        public IFormFile Picture { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [DataType(DataType.MultilineText)]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 40 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int? CategoryId { get; set; }

        public IEnumerable<Category>? Categories { get; set; }
        public int? AnimalId { get; set; }
    }
}
