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
                .WithOne()
                .HasForeignKey<SavedArticle>(sa => sa.ArticleID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.ArticleCategories)
                .WithOne(ac => ac.Category)
                .HasForeignKey(ac => ac.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
