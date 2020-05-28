using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

namespace MinecraftServerInfoPanel.Pages
{
    public class AboutModel : PageModel
    {
        public string Version { get; set; }

        public void OnGet()
        {
            Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
    }
}