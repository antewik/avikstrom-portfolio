using SendGrid;
using SendGrid.Helpers.Mail;

namespace WikstromIT.Services
{
    public class EmailService
    {
        private const string apiKey = "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";

        public static async Task<Response> SendEmail(string senderEmail, string recipientEmail, string subject, string body)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderEmail);
            var to = new EmailAddress(recipientEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            var response = await client.SendEmailAsync(msg);

            return response;
        }
    }
}