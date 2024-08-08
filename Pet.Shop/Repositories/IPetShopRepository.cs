using PetShop.Models;
using PetShop.Models.ViewModels;

namespace PetShop.Repositories
{
    public interface IPetShopRepository
    {
        IEnumerable<Animal> GetMostPopularAnimals(int animalNumber);
        IEnumerable<Animal> GetAllAnimals();
        IEnumerable<Category> GetAllCategories();
        Animal GetAnimalById(int animalId);
        void DeleteAnimal(int animalId);
        void AddComment(Comment comment);
        IEnumerable<string> GetAllCategoryNames();
        void AddCategory(Category category);
        void AddAnimal(Animal animal);
        Task UpdateAnimal(AnimalCreateVM model);
        Task<Comment> GetCommentById(int commentId);
        Task UpdateComment(Comment comment);
        Task DeleteCommentById(int commentId);
        void DeleteEmptyCategories();
        User AuthenticateUser(LoginVM userModel, out bool isPasswordWrong);
        bool IsUserTaken(string username);
        User RegisterUser(RegisterVM userModel);
        void SaveRefreshToken(string username, string refreshToken);
        string? GetRefreshToken(string username);
        User GetUserByUsername(string? username);
        void DeleteUserById(int userId);
        User UpdateAllUserDetails(UserUpdateVM userUpdateModel);
        User UpdateUserUsername(UserUpdateVM userUpdateModel);
        User UpdateUserPassword(UserUpdateVM userUpdateModel);
        User GetUserById(int userId);
        ICollection<Comment> GetAllCommentsOrderedByUsers();
        ICollection<Comment> GetAllCommentsOrderedByAnimals();
        void DeleteGuestComments();
        User GetGuestUser();
        Animal GetAnimalByIdWithDecryptedUsernames(int animalId);
        void DeleteUserCommentsById(int userId);
        ICollection<User> GetAllUsers(int userId);
    }
}
