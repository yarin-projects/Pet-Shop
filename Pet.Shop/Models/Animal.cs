using System.ComponentModel.DataAnnotations;

namespace PetShop.Models
{
    public class Animal
    {
        public int AnimalId { get; set; }
        
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^\p{L}+$", ErrorMessage = "Name can only contain letters.")]
        [MaxLength(15, ErrorMessage = "Name cannot be more than 15 characters.")]
        [MinLength(3, ErrorMessage = "Name has to be atleast 3 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 25, ErrorMessage = "Age has to be between 1-25.")]
        public byte Age { get; set; }

        [Required(ErrorMessage = "Picture is required.")]
        public string PictureName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [DataType(DataType.MultilineText)]
        [MaxLength(40, ErrorMessage = "Description cannot have more than 40 characters.")]
        [MinLength(3, ErrorMessage = "Description cannot have less than 3 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
