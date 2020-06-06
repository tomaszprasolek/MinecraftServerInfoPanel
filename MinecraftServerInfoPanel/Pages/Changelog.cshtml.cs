using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MinecraftServerInfoPanel.Pages
{
    [Authorize]
    public class ChangelogModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}