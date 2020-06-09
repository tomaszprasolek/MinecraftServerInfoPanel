using MinecraftServerInfoPanel.Database;
using MinecraftServerInfoPanel.Helpers;
using System;
using System.Linq;

namespace MinecraftServerInfoPanel.BL.PlayTimeCalculator
{
    public class PlayTimeCalculator : IPlayTimeCalculator
    {
        private readonly MinecraftDbContext dbContext;

        public PlayTimeCalculator(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TimeSpan CalculateUserAllPlayTime(string userName)
        {
            return CalculateUserPlayTime(userName, TimePeriod.AllTime, null);
        }

        public TimeSpan CalculateUserPlayTime(string userName, TimePeriod period, DateTime? date)
        {
            var logs = dbContext.ConsoleLogs
                .Where(x => x.Information.Contains(userName))
                .Where(x => x.Information.Contains("connected") ||
                            x.Information.Contains("disconnected"));

            if (period != TimePeriod.AllTime)
            {
                DateTime dateFrom = GetFromDate(period, date.Value);
                DateTime dateTo = GetToDate(period, date.Value);

                logs = logs.Where(x => x.Date > dateFrom && x.Date < dateTo);
            }

            var timeLog = logs
                .OrderByDescending(x => x.Date)
                .ToList();

            double totalMiliseconds = 0;
            DateTime playPerionEndTime = DateTime.MinValue;

            for (int i = 0; i < timeLog.Count; i++)
            {
                var log = timeLog[i];
                if (i == 0 && log.Information.Contains("Player connected")) continue;

                if (log.Information.Contains("disconnected"))
                {
                    playPerionEndTime = log.Date;
                    continue;
                }

                if (playPerionEndTime == DateTime.MinValue)
                    throw new Exception("playPerionEndTime is DateTime.MinValue");

                totalMiliseconds += (playPerionEndTime - log.Date).TotalMilliseconds;
            }

            return TimeSpan.FromMilliseconds(totalMiliseconds);
        }

        private DateTime GetFromDate(TimePeriod period, DateTime date)
        {
            switch (period)
            {
                case TimePeriod.Day:
                    return new DateTime(
                       date.Year,
                       date.Month,
                       date.Day,
                       0, 0, 0);
                case TimePeriod.Week:
                    return date.StartOfWeek(DayOfWeek.Monday);
                case TimePeriod.Month:
                    return new DateTime(date.Year, date.Month, 1);
                default:
                    throw new NotImplementedException();
            }
        }

        private DateTime GetToDate(TimePeriod period, DateTime date)
        {
            switch (period)
            {
                case TimePeriod.Day:
                    return new DateTime(
                       date.Year,
                       date.Month,
                       date.Day,
                       23, 59, 59);
                case TimePeriod.Week:
                    return date.EndOfWeek(DayOfWeek.Monday);
                case TimePeriod.Month:
                    return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
