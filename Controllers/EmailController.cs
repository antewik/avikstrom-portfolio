using Microsoft.AspNetCore.Mvc;
using System.Net;
using WikstromIT.Services;


namespace WikstromIT.Controllers
{
    public class EmailController : Controller
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string senderEmail, string recipientEmail, string subject, string body)
        {
            //await EmailService.SendEmail(senderEmail, recipientEmail, subject, body);
            //return RedirectToAction("Index");
            var response = await EmailService.SendEmail(senderEmail, recipientEmail, subject, body);
            
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                TempData["EmailSuccess"] = "TEST: Email sent successfully!";
            }

            return RedirectToAction("Index");
        }
    }
}
