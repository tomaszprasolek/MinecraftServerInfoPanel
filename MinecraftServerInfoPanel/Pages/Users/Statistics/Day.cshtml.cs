using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MinecraftServerInfoPanel.BL.PlayTimeCalculator;
using MinecraftServerInfoPanel.Database;
using System;

namespace MinecraftServerInfoPanel.Pages.Users.Statistics
{
    [Authorize]
    public class DayModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;
        private readonly IPlayTimeCalculator playTimeCalculator;

        // List to select
        public SelectList Users { get; set; }

        [BindProperty(SupportsGet = true)]
        public DayViewModel ViewModel { get; set; }

        public string PlayTime { get; set; }

        public DayModel(MinecraftDbContext dbContext, IPlayTimeCalculator playTimeCalculator)
        {
            this.dbContext = dbContext;
            this.playTimeCalculator = playTimeCalculator;
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
            PlayTime = playTimeCalculator.CalculateUserPlayTime(user.UserName, (TimePeriod)Enum.Parse(typeof(TimePeriod), ViewModel.Period, true), ViewModel.Date)
                .ToString(@"hh\:mm\:ss");

            return Page();
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
}