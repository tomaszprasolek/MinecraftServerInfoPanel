using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;

        [Required(ErrorMessage ="Hasło jest wymanage")]
        [Display(Name = "Hasło")]
        [BindProperty]
        public string Password { get; set; }

        public LoginModel(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        public void OnGet()
        {
            if (webHostEnvironment.IsDevelopment())
                Password = configuration["Password"];
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (Password != configuration["Password"])
            {
                ModelState.AddModelError("Password", "Nieprawidłowe hasło!");
                Password = string.Empty;
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, "Tomo"));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return LocalRedirect(returnUrl);
        }
    }
}