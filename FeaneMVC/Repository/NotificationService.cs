using System;
using System.Net;
using System.Net.Mail;
using WebApplication1.Interfaces;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Repository
{
    public class NotificationService : INotification
    {
        // Singleton instance
        private static readonly Lazy<NotificationService> _instance =
            new(() => new());

        private readonly IConfiguration _configuration;

        // Private constructor for Singleton pattern
        private NotificationService()
        {
            // Load configuration
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        // Public property to access the Singleton instance
        public static NotificationService Instance => _instance.Value;

        // Method to send notifications
        public void SendNotification(string message, string recipientEmail)
        {
            // Retrieve settings from configuration
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var password = _configuration["EmailSettings:Password"];

            // Create the email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Notification",
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(recipientEmail);

            // Configure the SMTP client
            using (var smtpClient = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = port == 587 // Commonly used for TLS
            })
            {
                try
                {
                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Notification sent successfully.");
                }
                catch (Exception ex)
                {
                    // Log error message
                    Console.WriteLine($"Error sending notification: {ex.Message}");
                }
            }
        }
    }
}
