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

        public List<UserDayStatisticsViewmodel> ViewModel { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Week { get; set; }

        [BindProperty(SupportsGet = true)]
        public TimePeriod Period { get; set; }

        //public string PreviousDay => Date.AddDays(-1).ToString("yyyy-MM-dd");
        //public string NextDay => Date.AddDays(1).ToString("yyyy-MM-dd");

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

            DateTime date;
            if (string.IsNullOrWhiteSpace(Week) == false)
            {
                var result = Week.Split("-W");
                date = ISOWeek.ToDateTime(Convert.ToInt32(result[0]), Convert.ToInt32(result[1]), DayOfWeek.Monday);
            }
            else
                date = DateTime.Now;

            ViewModel = users.Select(x => new UserDayStatisticsViewmodel
            {
                UserName = x.UserName,
                PlayTime = playTimeCalculator.CalculateUserPlayTime(x.UserName, Period, date)
            })
            .OrderByDescending(x => x.PlayTime)
            .ToList();

            return Page();
        }
    }
}