using Microsoft.AspNetCore.Mvc;

namespace WikstromIT.Controllers
{
    public class ResumeController : Controller
    {
        public IActionResult Index()
        {
            return PartialView();
        }

        public IActionResult ViewResume()
        {
            var resumeUrl = Url.Content("~/pdfs/Resume.pdf");
            return Redirect(resumeUrl);
        }
    }
}
