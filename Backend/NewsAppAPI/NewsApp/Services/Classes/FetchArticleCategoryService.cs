using NewsApp.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace NewsApp.Services.Classes
{
    public class FetchArticleCategoryService : IHostedService, IDisposable
    {

        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FetchArticleCategoryService> _logger;
        [ExcludeFromCodeCoverage]
        public FetchArticleCategoryService(IServiceScopeFactory scopeFactory, ILogger<FetchArticleCategoryService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        [ExcludeFromCodeCoverage]
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FetchArticlesTwoHourlyService is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(23));
            return Task.CompletedTask;
        }
        [ExcludeFromCodeCoverage]
        private void DoWork(object state)
        {
            _logger.LogInformation("FetchArticlesTwoHourlyService is working. Time: {time}", DateTimeOffset.Now);

            using (var scope = _scopeFactory.CreateScope())
            {
                var articleService = scope.ServiceProvider.GetRequiredService<IArticleService>();
                articleService.FetchAndSaveCategoryArticlesAsync().Wait();
            }

            _logger.LogInformation("FetchArticlesTwoHourlyService completed work. Time: {time}", DateTimeOffset.Now);
        }
        [ExcludeFromCodeCoverage]
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FetchArticlesTwoHourlyService is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
