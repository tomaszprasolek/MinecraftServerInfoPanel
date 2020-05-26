using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            this.logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.SignOutAsync();
            logger.LogInformation("Logout");
            return RedirectToPage("../Index");
        }
    }
}