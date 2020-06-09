using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MinecraftServerInfoPanel.Database;
using MinecraftServerInfoPanel.Helpers;
using System;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages.Users.Statistics
{
    [Authorize]
    public class DayModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        // List to select
        public SelectList Users { get; set; }

        // List to radio
        public string[] Periods { get; set; } = Enum.GetNames(typeof(TimePeriod));

        [BindProperty(SupportsGet = true)]
        public DayViewModel ViewModel { get; set; }

        public string PlayTime { get; set; }

        public DayModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult OnGet()
        {
            var user = dbContext.ServerUsers.Find(ViewModel.UserId);
            if (user == null)
            {
                return RedirectToPage("./NotFound");
            }

            Users = new SelectList(dbContext.ServerUsers, nameof(ServerUser.Id), nameof(ServerUser.UserName));

            if (ViewModel.Date == DateTime.MinValue) ViewModel.Date = DateTime.Now.Date;
            PlayTime = CalculateUserPlayTime(user.UserName, (TimePeriod)Enum.Parse(typeof(TimePeriod), ViewModel.Period, true), ViewModel.Date)
                .ToString(@"hh\:mm\:ss");

            return Page();
        }

        private TimeSpan CalculateUserPlayTime(string userName, TimePeriod period, DateTime? date)
        {
            DateTime dateFrom = GetFromDate(period, date.Value);
            DateTime dateTo = GetToDate(period, date.Value);

            var timeLog = dbContext.ConsoleLogs
                .Where(x => x.Information.Contains(userName))
                .Where(x => x.Information.Contains("connected") ||
                            x.Information.Contains("disconnected"))
                .Where(x => x.Date > dateFrom && x.Date < dateTo)
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

    public class DayViewModel
    {
        public int UserId { get; set; }

        public string Period { get; set; }

        public DateTime Date { get; set; }

        public string PreviousDay => Date.AddDays(-1).ToString("yyyy-MM-dd");

        public string NextDay => Date.AddDays(1).ToString("yyyy-MM-dd");
    }

    public enum TimePeriod
    {
        Day, Week, Month
    }
}