using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.Database;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages.Settings
{
    public class SettingsModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        [TempData]
        public string Message { get; set; }

        public List<Email> Emails { get; set; }

        public SettingsModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnGet()
        {
            Emails = dbContext.Emails.ToList();
        }
    }
}