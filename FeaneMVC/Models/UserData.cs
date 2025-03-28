using FinalProject.Models;
using System;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;
using WebApplication1.Models.Response;

namespace WebApplication1.Models
{
    public class UserData
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CartId { get; set; }
        public virtual Cart Cart { get; set; } // Навигационное свойство

        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Role Roles { get; set; } // Если предполагается поддержка множественных ролей, измените на List<Role>

        public string? Credential { get; set; }

        public bool IsActive { get; set; }

        public DateTime FirstRegisterTime { get; set; }

        public DateTime FirstLoginTime { get; set; }

        public string? IP { get; set; }

        public string? CookieValue { get; set; }

        public string? Address { get; set; } // Убедитесь, что это свойство требуется, если нужно

        [Phone]
        public string? PhoneNumber { get; set; }

        // Связь с DeliveryAddress
        public Guid DeliveryId { get; set; }
        public virtual DeliveryAddress Delivery { get; set; } // Навигационное свойство
    }
}
