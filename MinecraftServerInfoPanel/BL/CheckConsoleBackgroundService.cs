using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinecraftServerInfoPanel.BL.RecentActivityChecker;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL
{
    public class CheckConsoleBackgroundService : BackgroundService
    {
        private readonly ILogger<CheckConsoleBackgroundService> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public CheckConsoleBackgroundService(ILogger<CheckConsoleBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory
            )
        {
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested == false)
            {
                await DoWork();
                await Task.Delay(1_500_000, stoppingToken); // 1_500_000 ms == 25 min
            }
        }

        private async Task DoWork()
        {
            logger.LogInformation($"Start background service: CheckConsoleBackgroundService");

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var recentActivityChecker = scope.ServiceProvider
                    .GetRequiredService<IRecentActivityChecker>();

                await recentActivityChecker.CheckAsync();
            }

            logger.LogInformation($"End background service: CheckConsoleBackgroundService");
        }
    }
}
