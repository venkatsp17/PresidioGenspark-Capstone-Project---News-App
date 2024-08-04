using NewsApp.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace NewsApp.Services.Classes
{
    public class FetchArticlesService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FetchArticlesService> _logger;
        [ExcludeFromCodeCoverage]
        public FetchArticlesService(IServiceScopeFactory scopeFactory, ILogger<FetchArticlesService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        [ExcludeFromCodeCoverage]
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FetchArticlesService is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));
            return Task.CompletedTask;
        }
        [ExcludeFromCodeCoverage]
        private void DoWork(object state)
        {
            _logger.LogInformation("FetchArticlesService is working. Time: {time}", DateTimeOffset.Now);

            using (var scope = _scopeFactory.CreateScope())
            {
                var articleService = scope.ServiceProvider.GetRequiredService<IArticleService>();
                articleService.FetchAndSaveArticlesAsync().Wait();
            }

            _logger.LogInformation("FetchArticlesService completed work. Time: {time}", DateTimeOffset.Now);
        }
        [ExcludeFromCodeCoverage]
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FetchArticlesService is stopping.");
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
