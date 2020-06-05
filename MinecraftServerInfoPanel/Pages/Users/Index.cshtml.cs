using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages.Users
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        public List<ServerUserViewModel> Users { get; set; }

        public IndexModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnGet()
        {
            var users = dbContext.ServerUsers
                .Select(x => new { x.Id, x.UserName, x.Description })
                .ToList();

            Users = new List<ServerUserViewModel>(users.Count);

            for (int i = 0; i < users.Count; i++)
            {
                Users.Add(new ServerUserViewModel
                {
                    Id = users[i].Id,
                    Name = users[i].UserName,
                    PlayTime = CountUserPlayTime(users[i].UserName),
                    LastTimeOnServer = GetLastTimeOnServer(users[i].UserName),
                    Description = users[i].Description
                });
            }
        }

        private DateTime GetLastTimeOnServer(string userName)
        {
            return dbContext.ConsoleLogs
                .Where(x => x.Information.Contains(userName))
                .Where(x => x.Information.Contains("connected") &&
                         x.Information.Contains("disconnected") == false)
                .Max(x => x.Date);
        }

        private TimeSpan CountUserPlayTime(string userName)
        {
            var timeLog = dbContext.ConsoleLogs
                .Where(x => x.Information.Contains(userName))
                .Where(x => x.Information.Contains("connected") ||
                            x.Information.Contains("disconnected"))
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

    public class ServerUserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan PlayTime { get; set; }

        public string PlayTimeFriendly => PlayTime.ToString(@"hh\:mm\:ss");

        public DateTime LastTimeOnServer { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }
    }
}