using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinecraftServerInfoPanel.BL.RecentActivityChecker;
using System;
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
                logger.LogInformation($"[{DateTime.Now}] Start background task...");
                await DoWork();
                await Task.Delay(3_600_000, stoppingToken); // 3_600_000 ms == 1h
            }
        }

        private async Task DoWork()
        {
            logger.LogInformation($"[{DateTime.Now}] DoWork method start.");

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var recentActivityChecker = scope.ServiceProvider
                    .GetRequiredService<IRecentActivityChecker>();

                await recentActivityChecker.CheckAsync();
            }

            logger.LogInformation($"[{DateTime.Now}] DoWork method end.");
        }
    }
}
