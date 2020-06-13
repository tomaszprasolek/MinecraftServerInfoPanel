using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.BL.PlayTimeCalculator;
using MinecraftServerInfoPanel.Database;
using MinecraftServerInfoPanel.Models;
using MinecraftServerInfoPanel.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages.Users.Statistics
{
    public class WeekModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;
        private readonly IPlayTimeCalculator playTimeCalculator;

        public List<UserTimeViewModel> ViewModel { get; set; }

        [BindProperty(SupportsGet = true)]
        public Week Week { get; set; }

        public string PreviousWeek => Week.GetPreviousWeek().ToString();
        public string NextWeek => Week.GetNextWeek().ToString();

        public string DaysInWeekInfo => Week.GetDaysInWeekPeriod();

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

            if (Week == null)
                Week = Week.GetCurrentWeek();
            
            ViewModel = users.Select(x => new UserTimeViewModel
            {
                UserName = x.UserName,
                PlayTime = playTimeCalculator.CalculateUserPlayTime(x.UserName, TimePeriod.Week, Week.GetFirstDayOfWeek())
            })
            .OrderByDescending(x => x.PlayTime)
            .ToList();

            return Page();
        }
    }
}