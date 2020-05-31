using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.BL.EmailSender;
using MinecraftServerInfoPanel.Database;
using System.Linq;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.Pages.Settings
{
    public class SendTestEmailModel : PageModel
    {
        private readonly IEmailSender emailSender;
        private readonly MinecraftDbContext dbContext;

        public SendTestEmailModel(IEmailSender emailSender,
            MinecraftDbContext dbContext)
        {
            this.emailSender = emailSender;
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> OnPost()
        {
            var emails = dbContext.Emails.Select(x => x.EmailAddress).ToList();

            for (int i = 0; i < emails.Count; i++)
            {
                await emailSender.SendEmailAsync(emails[i], "Email testowy", "To jest email testowy. Proszę nie odpowiadać na niego");
            }

            TempData["Message"] = "Wysłano emaila testowego";
            return RedirectToPage("Index");
        }
    }
}