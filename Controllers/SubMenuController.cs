using Microsoft.AspNetCore.Mvc;

namespace WikstromIT.Controllers
{
    public class SubMenuController : Controller
    {
        public IActionResult LoadSubMenu(string type)
        {
            switch (type)
            {
                case "AboutMeMenu":
                    return PartialView("_AboutMeMenu");
                case "PortfolioMenu":
                    return PartialView("_PortfolioMenu");
                default:
                    return NoContent();
            }            
        }
    }
}
