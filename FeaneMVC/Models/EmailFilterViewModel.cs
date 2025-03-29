namespace FeaneMVC.Models
{
    public class EmailFilterViewModel
    {
        public List<string> Filters { get; set; }
        public List<MailMessage> Emails { get; set; }
        public Guid UserId { get; set; }
    }
}
