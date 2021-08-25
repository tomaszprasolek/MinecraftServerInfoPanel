using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.DataLayer;
using MinecraftServerInfoPanel.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages.Settings
{
    [Authorize]
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