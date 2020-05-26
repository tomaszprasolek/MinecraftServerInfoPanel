using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinecraftServerInfoPanel.BL.RecentActivityEmailSender;
using MinecraftServerInfoPanel.Database;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var consoleDataDowloader = scope.ServiceProvider
                    .GetRequiredService<IConsoleDataDowloader>();

                List<ConsoleLog> result = await consoleDataDowloader.Download();

                var dbContext = scope.ServiceProvider.GetRequiredService<MinecraftDbContext>();
                var maxDateInDb = dbContext.ConsoleLogs.Max(x => x.Date);

                var dbEntities = result
                    .Where(r => r.text.Contains("Running AutoCompaction...") == false)
                    .Select(r => new DbConsoleLog
                    {
                        Date = Convert.ToDateTime(r.text.Substring(1, 19)),
                        Information = r.text[26..].Trim(),
                        IsNeededToSendEmail = IsNeededToSendEmail(r.text)
                    })
                    .Where(r => r.Date > maxDateInDb)
                    .ToList();

                if (dbEntities.Count > 0)
                {
                    foreach (var entity in dbEntities)
                    {
                        dbContext.Entry(entity).State = EntityState.Added;
                        dbContext.ConsoleLogs.Add(entity);
                    }
                    dbContext.SaveChanges();
                }

                var recentActivityEmailSender = scope.ServiceProvider
                    .GetRequiredService<IRecentActivityEmailSender>();
                await recentActivityEmailSender.Send();
            }

            logger.LogInformation($"[{DateTime.Now}] DoWork method end.");
        }

        private bool IsNeededToSendEmail(string text) => text.Contains("connected");
    }
}
