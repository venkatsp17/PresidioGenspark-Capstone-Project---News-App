using Microsoft.EntityFrameworkCore;
using NewsApp.Models;

namespace NewsApp.Contexts
{
    public class NewsAppDBContext : DbContext
    {
        public NewsAppDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<SavedArticle> SavedArticles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ArticleCategory> ArticleCategories { get; set; }
        public DbSet<ShareData> ShareDatas { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleCategory>()
                .HasKey(ac => new { ac.ArticleID, ac.CategoryID });

            modelBuilder.Entity<Category>().HasData(
               new Category { CategoryID = 1, Name = "Israel-Hamas War", Description = "Israel-Hamas_War", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 2, Name = "Finance", Description = "FINANCE", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 3, Name = "Russia-Ukraine Conflict", Description = "Russia-Ukraine_Conflict", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 4, Name = "EURO 2024", Description = "EURO_2024", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 5, Name = "Explainers", Description = "EXPLAINERS", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 6, Name = "Lok Sabha Elections", Description = "LOK_SABHA_ELECTIONS", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 7, Name = "T20 WORLD CUP 2024", Description = "T20_WORLD_CUP_2024", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 8, Name = "Union Budget", Description = "UNION_BUDGET", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 9, Name = "Olympics 2024", Description = "OLYMPICS_2024", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 10, Name = "India", Description = "national", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 11, Name = "Business", Description = "business", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 12, Name = "Politics", Description = "politics", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 13, Name = "Sports", Description = "sports", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 14, Name = "Technology", Description = "technology", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 15, Name = "Startups", Description = "startup", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 16, Name = "Entertainment", Description = "entertainment", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 17, Name = "Hatke", Description = "hatke", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 18, Name = "International", Description = "world", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 19, Name = "Automobile", Description = "automobile", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 20, Name = "Science", Description = "science", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 21, Name = "Travel", Description = "travel", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 22, Name = "Miscellaneous", Description = "miscellaneous", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 23, Name = "Fashion", Description = "fashion", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 24, Name = "Education", Description = "education", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 25, Name = "Health & Fitness", Description = "Health___Fitness", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 26, Name = "Lifestyle", Description = "Lifestyle", Type = "NEWS_CATEGORY" },
               new Category { CategoryID = 27, Name = "Cricket", Description = "cricket", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 28, Name = "Top Stories", Description = "topstories", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 29, Name = "City", Description = "city", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 80, Name = "Experiment", Description = "experiment", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 81, Name = "Crime", Description = "crime", Type = "CUSTOM_CATEGORY" },
               new Category { CategoryID = 82, Name = "Feel_good_stories", Description = "Feel_Good_Stories", Type = "CUSTOM_CATEGORY" }
           );

            modelBuilder.Entity<User>()
                .HasMany(u => u.SavedArticles)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasMany(a => a.Comments)
                .WithOne(c => c.Article)
                .HasForeignKey(c => c.ArticleID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasMany(a => a.ArticleCategories)
                .WithOne(ac => ac.Article)
                .HasForeignKey(ac => ac.ArticleID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedArticle>()
                 .HasOne(sa => sa.Article)
                 .WithMany(a => a.SavedArticles) 
                 .HasForeignKey(sa => sa.ArticleID)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.ArticleCategories)
                .WithOne(ac => ac.Category)
                .HasForeignKey(ac => ac.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPreferences)
                .HasForeignKey(up => up.UserID);

            modelBuilder.Entity<UserPreference>()
                .HasOne(up => up.Category)
                .WithMany(c => c.UserPreferences)
                .HasForeignKey(up => up.CategoryID);

        }
    }
}
