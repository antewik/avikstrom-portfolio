using Microsoft.AspNetCore.Mvc;

namespace AvikstromPortfolio.Controllers
{
    public class BioController : Controller
    {
        private readonly ILogger<BioController> _logger;

        public BioController(ILogger<BioController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Bio page called");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "About_me.txt");

            try
            {
                var textContent = System.IO.File.ReadAllText(filePath);

                // Pass the text to the view
                ViewData["AboutMeText"] = textContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read About_me.txt at {Path}", filePath);

                // Log the exception (not implemented here)
                ViewData["AboutMeText"] = "Bio content is currently unavailable.";
            }

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