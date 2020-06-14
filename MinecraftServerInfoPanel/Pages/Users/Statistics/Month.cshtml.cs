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
    public class MonthModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;
        private readonly IPlayTimeCalculator playTimeCalculator;

        public List<UserTimeViewModel> ViewModel { get; set; }

        [BindProperty(SupportsGet = true)]
        public Month Month { get; set; }

        public string PreviousMonth => Month.GetPreviousMonth().ToString();
        public string NextMonth => Month.GetNextMonth().ToString();

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

            if (Month == null)
                Month = Month.GetCurrentMonth();

            ViewModel = users.Select(x => new UserTimeViewModel
            {
                UserName = x.UserName,
                PlayTime = playTimeCalculator.CalculateUserPlayTime(x.UserName, TimePeriod.Month, Month.GetFirstDayOfMonth())
            })
            .OrderByDescending(x => x.PlayTime)
            .ToList();

            return Page();
        }
    }
}