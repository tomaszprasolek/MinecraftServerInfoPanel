using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.Database;
using System.Linq;

namespace MinecraftServerInfoPanel.Pages.Settings
{
    public class DeleteEmailModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        public Email Email { get; set; }

        public DeleteEmailModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult OnGet(int id)
        {
            Email = dbContext.Emails.Where(x => x.Id == id).FirstOrDefault();
            if (Email == null)
            {
                return RedirectToPage("./NotFound");
            }
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var email = dbContext.Emails.Where(x => x.Id == id).FirstOrDefault();
            if (email != null)
            {
                dbContext.Remove(email);
                dbContext.SaveChanges();

                TempData["Message"] = $"Adres {email.EmailAddress} został usunięty.";
            }

            return RedirectToPage("Index");
        }
    }
}