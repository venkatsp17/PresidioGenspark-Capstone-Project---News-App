using DotNetEnv;
using log4net.Config;
using log4net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsApp.Contexts;
using NewsApp.Models;
using NewsApp.Repositories.Classes;
using NewsApp.Repositories.Interfaces;
using NewsApp.Services.Classes;
using NewsApp.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace NewsApp
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // Create a logger instance
            ILog log = LogManager.GetLogger(typeof(Program));
            log.Info("Application is starting.");

            Env.Load();


            builder.Configuration.AddEnvironmentVariables();
            log.Info("Adding Cors");
            #region cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://127.0.0.1:3000", "http://localhost:3000", "http://localhost", "http://127.0.0.1", "https://lively-mud-0c7af2f1e.5.azurestaticapps.net")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials(); // This line is crucial for allowing credentials
                    });
            });
            #endregion
            log.Info("Adding Controllers");
            builder.Services.AddControllers();
            log.Info("Adding SignalR");
            // Add SignalR
            builder.Services.AddSignalR();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            log.Info("Adding JWT Authentication");
            log.Error(builder.Configuration["JWT"]);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/commentHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

            });
            log.Info("Initalizing DB Context");
            #region context
            builder.Services.AddDbContext<NewsAppDBContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"))
                );
            #endregion
            log.Info("Adding Repositories & Services");
            #region Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
            builder.Services.AddScoped<IRepository<string, Comment, string>, CommentRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ISavedRepository, SavedArticleRepository>();
            builder.Services.AddScoped<IArticleCategoryRepository, ArticleCategoryRepository>();
            builder.Services.AddScoped<IRepository<string,ShareData,string>, ShareDataRepository>();
            builder.Services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
            #endregion

            #region Services
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IGoogleOAuthService, GoogleOAuthService>();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddLogging(config =>
            {
                config.AddConsole();
                config.AddDebug();
            });
            builder.Services.AddHostedService<FetchArticlesService>();
            builder.Services.AddHostedService<FetchArticleCategoryService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<ISavedArticleService, SavedArticleService>();
            builder.Services.AddScoped<IRankArticleService, RankArticleService>();
            builder.Services.AddScoped<IUserPreferenceService, UserPreferenceService>();


            #endregion

            builder.Services.AddHttpClient();

            var app = builder.Build();

            app.UseRouting();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<CommentHub>("/commentHub").RequireAuthorization();
            log.Info("Application Started successfully!");
            app.Run();
            log.Info("Application Ended!");
        }
    }
}
