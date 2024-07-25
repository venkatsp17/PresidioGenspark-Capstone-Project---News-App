using Microsoft.EntityFrameworkCore;
using NewsApp.Contexts;
using NewsApp.Models;
using NewsApp.Repositories.Classes;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Classes;
using NewsApp.Services.Interfaces;

namespace NewsApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();

            #region cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://127.0.0.1:3000", "http://localhost:3000", "http://localhost", "http://127.0.0.1")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
            #endregion

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            #region context
            builder.Services.AddDbContext<NewsAppDBContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"))
                );
            #endregion

            #region Repositories
            builder.Services.AddScoped<IRepository<string,User,string>, UserRepository>();
            builder.Services.AddScoped<IRepository<string, Article, string>, ArticleRepository>();
            builder.Services.AddScoped<IRepository<string, Comment, string>, CommentRepository>();
            builder.Services.AddScoped<IRepository<string, Category, string>, CategoryRepository>();
            builder.Services.AddScoped<IRepository<string, SavedArticle, string>, SavedArticleRepository>();
            builder.Services.AddScoped<IArticleCategoryRepository, ArticleCategoryRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();
            #endregion
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors("AllowSpecificOrigin");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
