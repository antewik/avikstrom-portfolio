using Microsoft.AspNetCore.Mvc;

namespace WikstromIT.Controllers
{
    public class BioController : Controller
    {
        public IActionResult Index()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "About_me.txt");
            var textContent = System.IO.File.ReadAllText(filePath);

            // Pass the text to the view
            ViewData["AboutMeText"] = textContent;

            return PartialView();
        }
    }
}
