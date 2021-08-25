using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.Database;
using MinecraftServerInfoPanel.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace MinecraftServerInfoPanel.Pages.Settings
{
    public class AddEmailModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        [Required(ErrorMessage = "Nie podano poprawnego adresu e-mail")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [BindProperty]
        public string NewEmail { get; set; }

        public AddEmailModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {
            dbContext.Emails.Add(new Email { EmailAddress = NewEmail });
            dbContext.SaveChanges();

            TempData["Message"] = "Dodano nowy adres email";
            return RedirectToPage("Index");
        }
    }
}