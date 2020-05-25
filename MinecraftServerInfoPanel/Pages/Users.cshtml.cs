using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages
{
    public class UsersModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        public List<User> Users { get; set; }

        public UsersModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnGet()
        {
            List<string> users = new List<string>(8);
            users.Add("bryszek12");
            users.Add("adwet8");
            users.Add("Prasolinho");
            users.Add("LenaPrasol");

            Users = new List<User>(users.Count);

            for (int i = 0; i < users.Count; i++)
            {
                Users.Add(new User { Name = users[i], PlayTime = CountUserPlayTime(users[i]) });
            }
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

    public class User
    {
        public string Name { get; set; }
        public TimeSpan PlayTime { get; set; }

        public string PlayTimeFriendly => PlayTime.ToString(@"hh\:mm\:ss");
    }
}