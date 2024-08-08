using Microsoft.EntityFrameworkCore;
using PetShop.Models;
using PetShop.Services.Encryption.AesEncryption;
using PetShop.Services.Encryption.Argon2Hashing;

namespace PetShop.Data
{
    public class PetShopContext(DbContextOptions<PetShopContext> options, IArgon2PasswordHasher passwordHasher, IAesEncryptionHelper encryptionHelper) : DbContext(options)
    {
        private readonly IArgon2PasswordHasher _passwordHasher = passwordHasher;
        private readonly IAesEncryptionHelper _encryptionHelper = encryptionHelper;

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Animal>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Animals)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Animal)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = -1, Name = "Admin" },
                new Role { RoleId = 1, Name = "Guest" },
                new Role { RoleId = 2, Name = "User" }
            );

            string encryptedAdminUsername = _encryptionHelper.Encrypt("admin");
            string adminPasswordHash = _passwordHasher.HashPassword("admin");
            string encryptedTestUsername = _encryptionHelper.Encrypt("test");
            string testUserPasswordHash = _passwordHasher.HashPassword("test");
            string guestUsername = _encryptionHelper.Encrypt("1");
            string guestPasswordHash = _passwordHasher.HashPassword("1");

            modelBuilder.Entity<User>().HasData(
                new User { UserId = -1, Username = encryptedAdminUsername, Password = adminPasswordHash, RoleId = -1 },
                new User { UserId = 1, Username = guestUsername, Password = guestPasswordHash, RoleId = 1 },
                new User { UserId = 2, Username = encryptedTestUsername, Password = testUserPasswordHash, RoleId = 2 }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Bird" },
                new Category { CategoryId = 2, Name = "Reptile" },
                new Category { CategoryId = 3, Name = "Mammal" },
                new Category { CategoryId = 4, Name = "Fish" },
                new Category { CategoryId = 5, Name = "Amphibian" },
                new Category { CategoryId = 6, Name = "Insect" }
            );
            modelBuilder.Entity<Animal>().HasData(
                new Animal { AnimalId = 1, Name = "Parrot", Age = 2, PictureName = "macaw1.png", Description = "Colorful and talkative parrot.", CategoryId = 1 },
                new Animal { AnimalId = 2, Name = "Iguana", Age = 4, PictureName = "iguana.png", Description = "Green iguana, loves basking in the sun.", CategoryId = 2 },
                new Animal { AnimalId = 3, Name = "Hamster", Age = 1, PictureName = "hamster.png", Description = "Cute and furry hamster.", CategoryId = 3 },
                new Animal { AnimalId = 4, Name = "Goldfish", Age = 1, PictureName = "goldfish.png", Description = "Beautiful goldfish swimming in the tank.", CategoryId = 4 },
                new Animal { AnimalId = 5, Name = "Frog", Age = 3, PictureName = "frog.png", Description = "Green tree frog, loves hopping around.", CategoryId = 5 },
                new Animal { AnimalId = 6, Name = "Butterfly", Age = 1, PictureName = "butterfly.png", Description = "Colorful butterfly fluttering around.", CategoryId = 6 },
                new Animal { AnimalId = 7, Name = "Canary", Age = 2, PictureName = "canary.png", Description = "Singing canary, bright yellow in color.", CategoryId = 1 },
                new Animal { AnimalId = 8, Name = "Python", Age = 5, PictureName = "python.png", Description = "Large python, very calm and docile.", CategoryId = 2 },
                new Animal { AnimalId = 9, Name = "Rabbit", Age = 2, PictureName = "rabbit1.png", Description = "Fluffy rabbit with long ears.", CategoryId = 3 },
                new Animal { AnimalId = 10, Name = "Betta Fish", Age = 1, PictureName = "bettafish.png", Description = "Betta fish with vibrant colors.", CategoryId = 4 },
                new Animal { AnimalId = 11, Name = "Salamander", Age = 4, PictureName = "salamander.png", Description = "Spotted salamander, loves moisture.", CategoryId = 5 },
                new Animal { AnimalId = 12, Name = "Ladybug", Age = 1, PictureName = "ladybug.png", Description = "Red ladybug with black spots.", CategoryId = 6 },
                new Animal { AnimalId = 13, Name = "Cockatoo", Age = 3, PictureName = "cockatoo.png", Description = "White cockatoo, very friendly.", CategoryId = 1 },
                new Animal { AnimalId = 14, Name = "Gecko", Age = 2, PictureName = "gecko.png", Description = "Small gecko, loves climbing walls.", CategoryId = 2 },
                new Animal { AnimalId = 15, Name = "Guinea Pig", Age = 1, PictureName = "guineapig.png", Description = "Cute guinea pig, loves eating veggies.", CategoryId = 3 }
            );
            modelBuilder.Entity<Comment>().HasData(
                new Comment { CommentId = 1, AnimalId = 7, Content = "Such a beautiful bird!", UserId = 1 },
                new Comment { CommentId = 2, AnimalId = 1, Content = "Very talkative and fun!", UserId = -1 },
                new Comment { CommentId = 3, AnimalId = 2, Content = "Amazing reptile, very calm.", UserId = 2 },
                new Comment { CommentId = 4, AnimalId = 3, Content = "So cute and small.", UserId = -1 },
                new Comment { CommentId = 5, AnimalId = 3, Content = "My kids love it!", UserId = 1 },
                new Comment { CommentId = 6, AnimalId = 3, Content = "Very easy to take care of.", UserId = 2 },
                new Comment { CommentId = 7, AnimalId = 4, Content = "Beautiful and relaxing to watch.", UserId = -1 },
                new Comment { CommentId = 8, AnimalId = 5, Content = "Jumps a lot, very lively.", UserId = 2 },
                new Comment { CommentId = 9, AnimalId = 6, Content = "So colorful and pretty.", UserId = 1 },
                new Comment { CommentId = 10, AnimalId = 7, Content = "Sings beautifully.", UserId = -1 },
                new Comment { CommentId = 11, AnimalId = 8, Content = "A bit scary, but very interesting.", UserId = -1 },
                new Comment { CommentId = 12, AnimalId = 9, Content = "Fluffy and friendly.", UserId = 2 },
                new Comment { CommentId = 13, AnimalId = 9, Content = "Great pet for kids.", UserId = 1 },
                new Comment { CommentId = 14, AnimalId = 10, Content = "So vibrant and colorful.", UserId = 1 },
                new Comment { CommentId = 15, AnimalId = 11, Content = "Loves moisture, very unique.", UserId = 2 },
                new Comment { CommentId = 16, AnimalId = 7, Content = "So small and cute.", UserId = 2 },
                new Comment { CommentId = 17, AnimalId = 13, Content = "Very friendly and social.", UserId = 1 },
                new Comment { CommentId = 18, AnimalId = 14, Content = "Great climber.", UserId = -1 },
                new Comment { CommentId = 19, AnimalId = 15, Content = "Loves eating veggies.", UserId = -1 },
                new Comment { CommentId = 20, AnimalId = 3, Content = "Very cute and cuddly.", UserId = 2 }
            );
        }
    }
}
