using System.ComponentModel.DataAnnotations;

namespace PetShop.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Required]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "Comment cannot be empty.")]
        [DataType(DataType.MultilineText)]
        [MaxLength(100, ErrorMessage = "Comment cannot have more than 100 characters.")]
        [MinLength(2, ErrorMessage = "Comment cannot have less than 2 characters.")]
        public string Content { get; set; }
        public virtual Animal Animal { get; set; }
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
