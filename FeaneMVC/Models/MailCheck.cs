using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace FeaneMVC.Models;

public class MailCheck
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Email { get; set; }  // Строковое поле

    public string Code { get; set; }
    public Guid UserId { get; set; }    // Внешний ключ к пользователю
    public UserData User { get; set; } // Навигационное свойство

}