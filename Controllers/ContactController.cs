using Microsoft.AspNetCore.Mvc;
using AvikstromPortfolio.Data;
using AvikstromPortfolio.Models.Contact;

namespace AvikstromPortfolio.Controllers
{
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly PortfolioDbContext _context;
        private static readonly Dictionary<string, DateTime> _ipSubmissionTimes = new();
        private static readonly TimeSpan _submissionCooldown = TimeSpan.FromMinutes(2);

        public ContactController(PortfolioDbContext context, ILogger<ContactController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Contact page loaded");

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                // For AJAX navigation: return only the partial view
                return PartialView();
            }

            // For direct navigation/refresh: return full view
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessage(ContactMessage model, string companyName)
        {
            string alertHtml;
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            _logger.LogInformation("SaveMessage called from IP: {IP}", ip);

            // Honeypot check
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                _logger.LogWarning("Honeypot triggered from IP: {IP}", ip);
                alertHtml = $"<div class='alert alert-danger mt-auto'>Your message could not be submitted. Please try again or <a href='mailto:avikstrom@wikstromIT.com'>contact</a> me directly.</div>";
                return Content(alertHtml, "text/html");
            }

            // Cooldown check
            if (_ipSubmissionTimes.TryGetValue(ip, out var lastSubmission))
            {
                if (DateTime.UtcNow - lastSubmission < _submissionCooldown)
                {
                    _logger.LogWarning("Cooldown active for IP: {IP}", ip);
                    alertHtml = $"<div class='alert alert-warning mt-auto'>Please wait a moment before submitting another message.</div>";
                    return Content(alertHtml, "text/html");
                }
            }

            // Message length check
            if (model.Body.Length > 1000)
            {
                _logger.LogWarning("Message too long from IP: {IP}, Length: {Length}", ip, model.Body.Length);
                alertHtml = $"<div class='alert alert-danger mt-auto'>Your message is too long. Please keep it under 1000 characters.</div>";
                return Content(alertHtml, "text/html");
            }

            try
            {
                var message = new ContactMessage
                {
                    SenderEmail = model.SenderEmail,
                    Subject = model.Subject,
                    Body = model.Body,
                    SubmittedAt = DateTime.UtcNow
                };

                await DbRetryHelper.ExecuteWithRetryAsync(async () =>
                {
                    _context.ContactMessages.Add(message);
                    await _context.SaveChangesAsync();
                }, _logger);

                _ipSubmissionTimes[ip] = DateTime.UtcNow;

                _logger.LogInformation("Message saved from {Email} with subject '{Subject}'", model.SenderEmail, model.Subject);

                alertHtml = $"<div class='alert alert-success mt-auto'>Thanks for reaching out! I’ve received your message and will get back to you as soon as possible.</div>";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving message from {Email}", model.SenderEmail);
                alertHtml = $"<div class='alert alert-danger mt-auto'>Sorry, something went wrong and your message couldn’t be saved. Please try again later or <a href='mailto:avikstrom@wikstromIT.com'>contact</a> me directly.</div>";
            }

            return Content(alertHtml, "text/html");
        }
    }
}
