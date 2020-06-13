using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.BL.PlayTimeCalculator;
using MinecraftServerInfoPanel.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages.Users.Statistics
{
    public class WeekModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;
        private readonly IPlayTimeCalculator playTimeCalculator;

        private Week selectedWeek;

        public List<UserDayStatisticsViewmodel> ViewModel { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Week { get; set; }

        public string PreviousWeek => selectedWeek.GetPreviousWeek().ToString();
        public string NextWeek => selectedWeek.GetNextWeek().ToString();

        public string DaysInWeekInfo => selectedWeek.GetDaysInWeekPeriod();

        public WeekModel(MinecraftDbContext dbContext, IPlayTimeCalculator playTimeCalculator)
        {
            this.dbContext = dbContext;
            this.playTimeCalculator = playTimeCalculator;
        }

        public IActionResult OnGet()
        {
            var users = dbContext.ServerUsers
                .Select(x => new { x.Id, x.UserName })
                .ToList();

            selectedWeek = Statistics.Week.ToWeek(Week);
            if (string.IsNullOrWhiteSpace(Week))
                Week = selectedWeek.ToString();

            ViewModel = users.Select(x => new UserDayStatisticsViewmodel
            {
                UserName = x.UserName,
                PlayTime = playTimeCalculator.CalculateUserPlayTime(x.UserName, TimePeriod.Week, selectedWeek.GetFirstDayOfWeek())
            })
            .OrderByDescending(x => x.PlayTime)
            .ToList();

            return Page();
        }
    }

    public class Week
    {
        public readonly int Year;
        public readonly int WeekNr;

        public Week(int year, int week)
        {
            Year = year;
            WeekNr = week;
        }

        public static Week ToWeek(string isoWeek)
        {
            if (string.IsNullOrWhiteSpace(isoWeek))
                return new Week(DateTime.Now.Year, ISOWeek.GetWeekOfYear(DateTime.Now));

            var result = isoWeek.Split("-W");
            return new Week(Convert.ToInt32(result[0]), Convert.ToInt32(result[1]));
        }

        public DateTime GetFirstDayOfWeek()
        {
            return ISOWeek.ToDateTime(Year, WeekNr, DayOfWeek.Monday);
        }

        public Week GetNextWeek()
        {
            int weeksInyear = ISOWeek.GetWeeksInYear(Year);
            int nextWeekNr = WeekNr + 1;

            if (nextWeekNr > weeksInyear)
                return new Week(Year + 1, 1);

            return new Week(Year, nextWeekNr);
        }

        public Week GetPreviousWeek()
        {
            if (WeekNr == 1)
                return new Week(Year - 1, ISOWeek.GetWeeksInYear(Year - 1));

            return new Week(Year, WeekNr - 1);
        }

        public string GetDaysInWeekPeriod()
        {
            var firstDayInWeek = GetFirstDayOfWeek();
            var lastDayInWeek = ISOWeek.ToDateTime(Year, WeekNr, DayOfWeek.Sunday);

            if (firstDayInWeek.Year != lastDayInWeek.Year)
                return $"{firstDayInWeek:dd.MM.yyyy} - {lastDayInWeek:dd.MM.yyyy}";

            return $"{firstDayInWeek:dd.MM} - {lastDayInWeek:dd.MM}.{Year}";
        }

        public override string ToString()
        {
            return $"{Year}-W{WeekNr}";
        }
    }
}