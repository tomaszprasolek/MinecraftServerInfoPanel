﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages
{
    [Authorize]
    public class LogsModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        public List<LogViewModel> Logs { get; set; }

        public LogsModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnGet()
        {
            Logs = dbContext.Logs
                .Select(x => new LogViewModel
                {
                    Message = x.Message,
                    Level = x.Level,
                    Date = x.TimeStamp.DateTime,
                    Exception = x.Exception
                })
                .ToList();
        }
    }

    public class LogViewModel
    {
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime Date { get; set; }
        public string Exception { get; set; }
    }
}