namespace PetShop.Models.ViewModels
{
    public class AdminProfileVM
    {
        public User User { get; set; }
        public ICollection<Comment> CommentsOrderedByAnimal { get; set; } = new HashSet<Comment>();
        public ICollection<Comment> CommentsOrderedByUser { get; set; } = new HashSet<Comment>();
        public ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
