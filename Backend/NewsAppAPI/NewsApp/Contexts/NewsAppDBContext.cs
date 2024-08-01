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
