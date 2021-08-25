using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MinecraftServerInfoPanel.DataLayer;

namespace MinecraftServerInfoPanel.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly MinecraftDbContext dbContext;

        [BindProperty]
        public ServerUserViewModel ViewModel { get; set; }

        public EditModel(MinecraftDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult OnGet(int id)
        {
            var serverUser = dbContext.ServerUsers.Find(id);
            if (serverUser == null)
            {
                return RedirectToPage("./NotFound");
            }

            ViewModel = new ServerUserViewModel
            {
                Id = serverUser.Id,
                Name = serverUser.UserName,
                Description = serverUser.Description
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var serverUser = dbContext.ServerUsers.Find(ViewModel.Id);
            if (serverUser == null)
            {
                return RedirectToPage("./NotFound");
            }

            serverUser.Description = ViewModel.Description;
            dbContext.SaveChanges();

            return RedirectToPage("Index");
        }
    }
}