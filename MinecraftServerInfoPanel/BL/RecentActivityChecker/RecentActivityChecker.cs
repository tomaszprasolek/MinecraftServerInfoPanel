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

            var dbEntities = ConvertToDbConsoleLog(result)
                .Where(r => r.Date > maxDateInDb)
                .ToList();

            bool newActivitiesOnServer = false;

            if (dbEntities.Count > 0)
            {
                AddServerLogsToDb(dbEntities);
                newActivitiesOnServer = true;
            }

            await recentActivityEmailSender.Send();

            return newActivitiesOnServer;
        }

        private IEnumerable<DbConsoleLog> ConvertToDbConsoleLog(List<ConsoleLog> list)
        {
            return list
                .Where(r => r.text.Contains("Running AutoCompaction...") == false)
                .Select(r => new DbConsoleLog
                {
                    Date = Convert.ToDateTime(r.text.Substring(1, 19)),
                    Information = r.text[26..].Trim(),
                    IsNeededToSendEmail = IsNeededToSendEmail(r.text)
                });
        }

        private void AddServerLogsToDb(List<DbConsoleLog> serverlogs)
        {
            foreach (var entity in serverlogs)
            {
                dbContext.Entry(entity).State = EntityState.Added;
                dbContext.ConsoleLogs.Add(entity);
            }
            dbContext.SaveChanges();
        }

        private bool IsNeededToSendEmail(string text) => text.Contains("connected");
    }
}
