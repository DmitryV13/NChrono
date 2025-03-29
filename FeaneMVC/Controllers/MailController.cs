using FeaneMVC.Models;
using FinalProject.DbModel;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using FolderAccess = MailKit.FolderAccess;
using MessageSummaryItems = MailKit.MessageSummaryItems;

namespace FeaneMVC.Controllers;

[ApiController]
[Route("api/filters")]
public class MailController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly WebApplication1.Interfaces.ISession _sessionService;

    public MailController(ApplicationDbContext context, WebApplication1.Interfaces.ISession sessionService)
    {
        _context = context;
        _sessionService = sessionService;
    }

    [HttpGet("read-emails")]
    public IActionResult ReadEmails()
    {
        var userId = _sessionService.GetUserId();
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        var userFilters = _context.UserFilters
            .Where(f => f.UserId == userId)
            .Select(f => f.Filter)
            .ToList();

        foreach (var filter in userFilters)
        {
            string email = "rentshopvehicle.webapplication@mail.ru";
            string password = "BKRZTN085g5qRbxAmhhT"; 
            string imapServer = "imap.mail.ru";
            int imapPort = 993;

            try
            {
                using (var client = new ImapClient())
                {
                    client.Connect(imapServer, imapPort, SecureSocketOptions.SslOnConnect);
                    client.Authenticate(email, password);

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);

                    var messages = inbox.Fetch(0, -1, 
                        MessageSummaryItems.Full | MessageSummaryItems.UniqueId | MessageSummaryItems.Flags)
                        .Where(msg => (msg.Flags & MessageFlags.Seen) == 0) 
                        .ToList();

                    foreach (var message in messages)
                    {
                        var fullMessage = inbox.GetMessage(message.UniqueId);
                        string uniqueId = message.UniqueId.ToString();

                        bool mailExists = _context.MailMessages.Any(m => m.UniqueId == uniqueId && m.UserId == userId);

                        if (mailExists)
                            continue;

                        var mailMessage = new MailMessage
                        {
                            UserId = userId,
                            Email = fullMessage.From.ToString(),
                            Subject = fullMessage.Subject,
                            Body = fullMessage.TextBody ?? fullMessage.HtmlBody,
                            Date = fullMessage.Date.UtcDateTime,
                            UniqueId = uniqueId,
                            Filter = filter
                        };

                        _context.MailMessages.Add(mailMessage);
                        _context.SaveChanges();

                        Console.WriteLine($"[INFO] Письмо от {mailMessage.Email} с темой '{mailMessage.Subject}' сохранено.");
                    }

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return StatusCode(500, "Ошибка при получении писем.");
            }
        }

        return Ok("Письма успешно обработаны.");
    }

    [HttpGet("user-mails")]
    public IActionResult GetUserMails()
    {
        var userId = _sessionService.GetUserId();
        var mails = _context.MailMessages
            .Where(m => m.UserId == userId)
            .ToList();

        return Ok(mails);
    }
    
    [HttpGet("emails")]

    public IActionResult Index()
    {

        var userId = _sessionService.GetUserId();
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        // Получаем фильтры пользователя
        var userFilters = _context.UserFilters
            .Where(f => f.UserId == userId)
            .Select(f => f.Filter)
            .Distinct()
            .ToList();

        // Получаем все письма пользователя
        var mails = _context.MailMessages
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.Date)
            .ToList();

        // Создаем модель представления
        var viewModel = new EmailFilterViewModel
        {
            Filters = userFilters,
            Emails = mails,
            UserId = userId
        };

        return View(viewModel); 
    }
    
}
