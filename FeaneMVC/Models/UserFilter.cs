using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

namespace FeaneMVC.Models;

public sealed class UserFilter
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Filter { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public UserData User { get; set; }
}