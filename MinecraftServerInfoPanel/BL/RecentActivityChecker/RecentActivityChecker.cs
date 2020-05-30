using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MinecraftServerInfoPanel.BL.RecentActivityEmailSender;
using MinecraftServerInfoPanel.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL.RecentActivityChecker
{

    public class RecentActivityChecker : IRecentActivityChecker
    {
        private readonly ILogger<RecentActivityChecker> logger;
        private readonly IConsoleDataDowloader consoleDataDowloader;
        private readonly MinecraftDbContext dbContext;
        private readonly IRecentActivityEmailSender recentActivityEmailSender;

        public RecentActivityChecker(ILogger<RecentActivityChecker> logger,
            IConsoleDataDowloader consoleDataDowloader,
            MinecraftDbContext dbContext,
            IRecentActivityEmailSender recentActivityEmailSender)
        {
            this.logger = logger;
            this.consoleDataDowloader = consoleDataDowloader;
            this.dbContext = dbContext;
            this.recentActivityEmailSender = recentActivityEmailSender;
        }

        public async Task<bool> CheckAsync()
        {
            List<ConsoleLog> result = await consoleDataDowloader.Download();

            DateTime maxDateInDb;
            if (dbContext.ConsoleLogs.Count() == 0)
                maxDateInDb = DateTime.MinValue;
            else
                maxDateInDb = dbContext.ConsoleLogs.Max(x => x.Date);

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

            bool newActivitiesOnServer = false;

            if (dbEntities.Count > 0)
            {
                foreach (var entity in dbEntities)
                {
                    dbContext.Entry(entity).State = EntityState.Added;
                    dbContext.ConsoleLogs.Add(entity);
                }
                dbContext.SaveChanges();
                newActivitiesOnServer = true;
            }

            await recentActivityEmailSender.Send();

            return newActivitiesOnServer;
        }

        private bool IsNeededToSendEmail(string text) => text.Contains("connected");
    }
}
