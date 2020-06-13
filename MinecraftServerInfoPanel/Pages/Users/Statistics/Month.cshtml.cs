using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.BL.PlayTimeCalculator;
using MinecraftServerInfoPanel.Database;
using MinecraftServerInfoPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages.Users.Statistics
{
    public class MonthModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;
        private readonly IPlayTimeCalculator playTimeCalculator;

        private Month selectedMonth;

        public List<UserTimeViewModel> ViewModel { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Month { get; set; }

        public string PreviousMonth => selectedMonth.GetPreviousMonth().ToString();
        public string NextMonth => selectedMonth.GetNextMonth().ToString();

        public MonthModel(MinecraftDbContext dbContext, IPlayTimeCalculator playTimeCalculator)
        {
            this.dbContext = dbContext;
            this.playTimeCalculator = playTimeCalculator;
        }

        public IActionResult OnGet()
        {
            var users = dbContext.ServerUsers
                .Select(x => new { x.Id, x.UserName })
                .ToList();

            selectedMonth = Statistics.Month.ToMonth(Month);
            if (string.IsNullOrWhiteSpace(Month))
                Month = selectedMonth.ToString();

            ViewModel = users.Select(x => new UserTimeViewModel
            {
                UserName = x.UserName,
                PlayTime = playTimeCalculator.CalculateUserPlayTime(x.UserName, TimePeriod.Month, selectedMonth.GetFirstDayOfMonth())
            })
            .OrderByDescending(x => x.PlayTime)
            .ToList();

            return Page();
        }
    }

    public class Month
    {
        public int Year { get; }
        public int MonthNr { get; }

        public Month(int year, int month)
        {
            Year = year;
            MonthNr = month;
        }

        public static Month ToMonth(string yearMonth)
        {
            if (string.IsNullOrWhiteSpace(yearMonth))
                return new Month(DateTime.Now.Year, DateTime.Now.Month);

            var result = yearMonth.Split("-");
            return new Month(Convert.ToInt32(result[0]), Convert.ToInt32(result[1]));
        }

        public Month GetPreviousMonth()
        {
            if (MonthNr == 1)
                return new Month(Year - 1, 12);

            return new Month(Year, MonthNr - 1);
        }

        public Month GetNextMonth()
        {
            if (MonthNr == 12)
                return new Month(Year + 1, 1);

            return new Month(Year, MonthNr + 1);
        }

        public DateTime GetFirstDayOfMonth()
        {
            return new DateTime(Year, MonthNr, 1);
        }

        public override string ToString()
        {
            return $"{Year}-{MonthNr.ToString().PadLeft(2, '0')}";
        }
    }
}