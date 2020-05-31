using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MinecraftServerInfoPanel.BL.RecentActivityEmailSender;
using MinecraftServerInfoPanel.Database;
using MinecraftServerInfoPanel.Helpers;
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

            CheckServerUsers(dbEntities);

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

        private void CheckServerUsers(List<DbConsoleLog> serverlogs)
        {
            var distinctUsers = serverlogs
                .Select(logEntry =>
                {
                    var idx = logEntry.Information.IndexOf("connected: ") + 11;
                    var idxLast = logEntry.Information.IndexOf(", xuid");

                    string userName = logEntry.Information.Substring(idx, idxLast - idx).Trim();
                    string xuid = logEntry.Information.Substring(logEntry.Information.Length - 16, 16).Trim();

                    return new ServerUser { UserName = userName, Xuid = xuid };
                })
                .DistinctBy(x => x.Xuid);

            var usersInDb = dbContext.ServerUsers.ToList();

            var newUsers = distinctUsers.Select(x => x.UserName)
                .Except(usersInDb.Select(y => y.UserName))
                .ToList();

            if (newUsers.Count > 0)
            {
                for (int i = 0; i < newUsers.Count; i++)
                {
                    string userName = newUsers[i];
                    string xuid = distinctUsers.Where(x => x.UserName == newUsers[i]).Select(x => x.Xuid).FirstOrDefault();
                    dbContext.ServerUsers.Add(new ServerUser { UserName = userName, Xuid = xuid });
                }
                dbContext.SaveChanges();
            }
        }


        private bool IsNeededToSendEmail(string text) => text.Contains("connected");
    }
}
