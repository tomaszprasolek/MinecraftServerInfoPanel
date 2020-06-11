﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.BL.PlayTimeCalculator;
using MinecraftServerInfoPanel.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages.Users.Statistics
{
    [Authorize]
    public class DayModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;
        private readonly IPlayTimeCalculator playTimeCalculator;

        public List<UserDayStatisticsViewmodel> ViewModel { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime Date { get; set; }

        [BindProperty(SupportsGet = true)]
        public TimePeriod Period { get; set; }

        public string PreviousDay => Date.AddDays(-1).ToString("yyyy-MM-dd");
        public string NextDay => Date.AddDays(1).ToString("yyyy-MM-dd");

        public DayModel(MinecraftDbContext dbContext, IPlayTimeCalculator playTimeCalculator)
        {
            this.dbContext = dbContext;
            this.playTimeCalculator = playTimeCalculator;
        }

        public IActionResult OnGet()
        {
            var users = dbContext.ServerUsers
                .Select(x => new { x.Id, x.UserName })
                .ToList();

            if (Date == DateTime.MinValue) Date = DateTime.Now.Date;

            ViewModel = users.Select(x => new UserDayStatisticsViewmodel
            {
                UserName = x.UserName,
                PlayTime = playTimeCalculator.CalculateUserPlayTime(x.UserName, Period, Date).ToString(@"hh\:mm\:ss")
            })
            .ToList();

            return Page();
        }
    }

    public class UserDayStatisticsViewmodel
    {
        public string UserName { get; set; }

        public string PlayTime { get; set; }
    }
}