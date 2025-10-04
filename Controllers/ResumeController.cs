using Microsoft.AspNetCore.Mvc;

namespace AvikstromPortfolio.Controllers
{
    public class ResumeController : Controller
    {
        public IActionResult Index()
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                // For AJAX navigation: return only the partial view
                return PartialView();
            }

            // For direct navigation/refresh: return full view
            return View();
        }
    }
}
