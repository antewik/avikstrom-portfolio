namespace AvikstromPortfolio.Models.Contact
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public string SenderEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }

}
