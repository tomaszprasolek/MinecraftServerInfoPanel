using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration configuration;

        [Required(ErrorMessage ="Hasło jest wymanage")]
        [Display(Name = "Hasło")]
        [BindProperty]
        public string Password { get; set; }

        public LoginModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Password != configuration["Password"])
            {
                ModelState.AddModelError("Password", "Nieprawidłowe hasło!");
                return Page();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, "Tomo"));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("../Index");
        }
    }
}