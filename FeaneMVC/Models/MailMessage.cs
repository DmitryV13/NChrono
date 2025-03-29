using System.ComponentModel.DataAnnotations;

namespace FeaneMVC.Models;

public class MailMessage
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } 
    public string Subject { get; set; } 
    public string Body { get; set; } 
    public DateTime Date { get; set; } 
    public string UniqueId { get; set; } 
    public string Filter { get; set; }
}