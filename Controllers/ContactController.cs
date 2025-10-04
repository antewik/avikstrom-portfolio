using Microsoft.AspNetCore.Mvc;
using System.Net;
using WikstromIT.Data;
using WikstromIT.Models.Contact;

namespace WikstromIT.Controllers
{
    public class ContactController : Controller
    {
        private readonly PortfolioDbContext _context;
        private static readonly Dictionary<string, DateTime> _ipSubmissionTimes = new();
        private static readonly TimeSpan _submissionCooldown = TimeSpan.FromMinutes(2);

        public ContactController(PortfolioDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessage(string senderEmail, string subject, string body, string companyName)
        {
            string alertHtml;

            // Honeypot check
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                alertHtml = $"<div class='alert alert-danger mt-auto'>Your message could not be submitted. Please try again or <a href='mailto:avikstrom@wikstromIT.com'>contact</a> me directly.</div>";
                return Content(alertHtml, "text/html");
            }

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Cooldown check
            if (_ipSubmissionTimes.TryGetValue(ip, out var lastSubmission))
            {
                if (DateTime.UtcNow - lastSubmission < _submissionCooldown)
                {
                    alertHtml = $"<div class='alert alert-warning mt-auto'>Please wait a moment before submitting another message.</div>";
                    return Content(alertHtml, "text/html");
                }
            }

            // Message length check
            if (body.Length > 1000)
            {
                alertHtml = $"<div class='alert alert-danger mt-auto'>Your message is too long. Please keep it under 1000 characters.</div>";
                return Content(alertHtml, "text/html");
            }

            try
            {
                var message = new ContactMessage
                {
                    SenderEmail = senderEmail,
                    Subject = subject,
                    Body = body,
                    SubmittedAt = DateTime.UtcNow
                };

                _context.ContactMessages.Add(message);
                await _context.SaveChangesAsync();

                _ipSubmissionTimes[ip] = DateTime.UtcNow;

                alertHtml = $"<div class='alert alert-success mt-auto'>Thanks for reaching out! I’ve received your message and will get back to you as soon as possible.</div>";
            }
            catch (Exception ex)
            {
                alertHtml = $"<div class='alert alert-danger mt-auto'>Sorry, something went wrong and your message couldn’t be saved. Please try again later or <a href='mailto:avikstrom@wikstromIT.com'>contact</a> me directly.</div>";
                // Optional: log ex.Message or ex.ToString() for diagnostics
            }

            return Content(alertHtml, "text/html");
        }

    }
}
