using Microsoft.AspNetCore.Mvc;

namespace MinecraftServerInfoPanel.ViewComponents
{
    public class UserStatistcsMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
