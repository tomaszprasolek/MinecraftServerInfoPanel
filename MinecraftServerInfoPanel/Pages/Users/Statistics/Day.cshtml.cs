using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MinecraftServerInfoPanel.Database;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace MinecraftServerInfoPanel.Pages.Users.Statistics
{
    [Authorize]
    public class DayModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        public string UserName { get; set; }
        public string PlayTime { get; set; }

        public SelectList Users { get; set; }


        public DayModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult OnGet(int userId, DateTime? date)
        {
            var user = dbContext.ServerUsers.Find(userId);
            if (user == null)
            {
                return RedirectToPage("./NotFound");
            }

            Users = new SelectList(dbContext.ServerUsers, nameof(ServerUser.Id), nameof(ServerUser.UserName));

            PlayTime = CalculateUserPlayTime(user.UserName, date).ToString(@"hh\:mm\:ss");
            UserName = user.UserName;

            return Page();
        }

        private TimeSpan CalculateUserPlayTime(string userName, DateTime? date)
        {
            if (date == null) date = DateTime.Now;

            DateTime dateFrom = new DateTime(
               date.Value.Year,
               date.Value.Month,
               date.Value.Day,
               0, 0, 0);

            DateTime dateTo = new DateTime(
                date.Value.Year,
                date.Value.Month,
                date.Value.Day,
                23, 59, 59);

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
    }
}