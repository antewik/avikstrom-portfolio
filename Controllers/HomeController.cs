using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AvikstromPortfolio.Models;

namespace AvikstromPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? code = null)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Error occurred. Code: {Code}, RequestId: {RequestId}", code, requestId);

            var model = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = code
            };

            return View(model);
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping() => Ok("Web Application is awake");
    }
}
