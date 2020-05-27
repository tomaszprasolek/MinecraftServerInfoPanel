using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.Database;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages
{
    public class LogsModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        public List<Log> Logs { get; set; }

        public LogsModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnGet()
        {
            Logs = dbContext.Logs.OrderByDescending(x => x.TimeStamp).Take(100).ToList();
        }
    }
}