using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MinecraftServerInfoPanel.BL.RecentActivityChecker;
using MinecraftServerInfoPanel.DataLayer;
using MinecraftServerInfoPanel.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MinecraftServerInfoPanel.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IRecentActivityChecker recentActivityChecker;
        private readonly MinecraftDbContext dbContext;

        public List<ServerLogViewModel> Logs { get; set; }

        [TempData]
        public string Message { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            MinecraftDbContext dbContext,
            IRecentActivityChecker recentActivityChecker)
        {
            _logger = logger;
            this.recentActivityChecker = recentActivityChecker;
            this.dbContext = dbContext;
        }

        public void OnGet()
        {
            _logger.LogInformation("Get all saved Minecraft server logs");

            Logs = dbContext.ConsoleLogs
                .Select(x => new ServerLogViewModel
                {
                    Date = x.Date,
                    Information = HttpUtility.HtmlDecode(x.Information)
                })
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await recentActivityChecker.CheckAsync();

            TempData["Message"] = result ? "Pobrano nowe wpisy z serwera." : "Nie pobrano żadnych danych z serwera.";

            return RedirectToPage("Index");
        }
    }
}
