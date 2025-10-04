using Microsoft.AspNetCore.Mvc;

namespace WikstromIT.Controllers
{
    public class ResumeController : Controller
    {
        public IActionResult Index()
        {
            return PartialView();
        }
    }
}
