using System.ComponentModel.DataAnnotations;

namespace PetShop.Models.ViewModels
{
    public class CommentCreateVM
    {
        [Required]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "Comment cannot be empty.")]
        [DataType(DataType.MultilineText)]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Comment must be between 2 and 100 characters long.")]
        public string Content { get; set; }

        public int? CommentId { get; set; }
    }
}
