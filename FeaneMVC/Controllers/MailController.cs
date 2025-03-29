using FeaneMVC.Models;
using FinalProject.DbModel; // Предполагаю, что это ваш контекст БД (ApplicationDbContext)
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FolderAccess = MailKit.FolderAccess;
using MessageSummaryItems = MailKit.MessageSummaryItems;

namespace FeaneMVC.Controllers
{
    [ApiController]
    [Route("api/filters")]
    public class MailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly WebApplication1.Interfaces.ISession _sessionService;
        private readonly IHttpClientFactory _httpClientFactory; // Добавлено для работы с HTTP-запросами
        private readonly string _chatApiKey; // Ключ API для чата
        private readonly string _chatBaseUrl; // Базовый URL API чата

        public MailController(
            ApplicationDbContext context,
            WebApplication1.Interfaces.ISession sessionService,
            IHttpClientFactory httpClientFactory, // Добавлено
            IConfiguration configuration) // Добавлено для получения конфигурации
        {
            _context = context;
            _sessionService = sessionService;
            _httpClientFactory = httpClientFactory;
            _chatApiKey = configuration["ChatApi:Key"] ?? throw new ArgumentException("Chat API key is not configured.");
            _chatBaseUrl = configuration["ChatApi:BaseUrl"] ?? throw new ArgumentException("Chat API base URL is not configured.");
        }

        [HttpGet("read-emails")]
        public async Task<IActionResult> ReadEmails()
        {
                var userId = _sessionService.GetUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Получаем все фильтры пользователя один раз перед синхронизацией
            var userFilters = _context.UserFilters
                .Where(f => f.UserId == userId)
                .Select(f => f.Filter)
                .Distinct()
                .ToList();

            string email = "vistovskii2002@mail.ru";
            string password = "qg6fcfBMn0eWNbyg6Vyr";
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

                        // Проверяем, существует ли письмо в базе
                        bool mailExists = _context.MailMessages.Any(m => m.UniqueId == uniqueId && m.UserId == userId);

                        if (mailExists)
                            continue;

                        // Формируем текст для анализа (тема + тело письма)
                        string messageText = $"{fullMessage.Subject} {fullMessage.TextBody ?? fullMessage.HtmlBody}";

                        // Определяем наиболее подходящий фильтр с помощью AI
                        string matchedFilter = await GetBestMatchingFilter(userFilters, messageText);

                        var mailMessage = new MailMessage
                        {
                            UserId = userId,
                            Email = fullMessage.From.ToString(),
                            Subject = fullMessage.Subject ?? "Filtration",
                            Body = fullMessage.TextBody ?? fullMessage.HtmlBody,
                            Date = fullMessage.Date.UtcDateTime,
                            UniqueId = uniqueId,
                            Filter = matchedFilter ?? "Uncategorized" // Если фильтр не определен, используем "Uncategorized"
                        };

                        _context.MailMessages.Add(mailMessage);
                        _context.SaveChanges();

                        Console.WriteLine($"[INFO] Письмо от {mailMessage.Email} с темой '{mailMessage.Subject}' сохранено с фильтром '{mailMessage.Filter}'.");
                    }

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return StatusCode(500, "Ошибка при получении писем.");
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

            var userFilters = _context.UserFilters
                .Where(f => f.UserId == userId)
                .Select(f => f.Filter)
                .Distinct()
                .ToList();

            var mails = _context.MailMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.Date)
                .ToList();

            var viewModel = new EmailFilterViewModel
            {
                Filters = userFilters,
                Emails = mails,
                UserId = userId
            };

            return View(viewModel);
        }

        /// <summary>
        /// Определяет наиболее подходящий фильтр для текста сообщения из предоставленного списка фильтров
        /// </summary>
        /// <param name="filters">Список всех доступных фильтров</param>
        /// <param name="messageText">Текст сообщения для анализа</param>
        /// <returns>Наиболее подходящий фильтр или null в случае ошибки</returns>
        private async Task<string> GetBestMatchingFilter(List<string> filters, string messageText)
{
    if (filters == null || !filters.Any() || string.IsNullOrWhiteSpace(messageText))
    {
        return null;
    }

    var client = _httpClientFactory.CreateClient();
    client.BaseAddress = new Uri(_chatBaseUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _chatApiKey);

    var prompt = $"У меня есть список фильтров: {string.Join(", ", filters)}. " +
                $"Проанализируй следующий текст сообщения и выбери один фильтр из списка, " +
                $"который лучше всего ему соответствует. Верни результат в виде JSON-массива с одним элементом, " +
                $"содержащим только название фильтра без пояснений. " +
                $"Текст сообщения: '{messageText}'";

    var payload = new
    {
        model = "gpt-3.5-turbo",
        messages = new[]
        {
            new { role = "system", content = "Твоя задача — выбрать один наиболее подходящий фильтр из предоставленного списка для данного текста. Верни результат в виде JSON-массива с одним элементом, содержащим только название фильтра." },
            new { role = "user", content = prompt }
        },
        max_tokens = 50
    };

    var jsonPayload = JsonSerializer.Serialize(payload);
    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    try
    {
        var response = await client.PostAsync("chat/completions", content);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"API request failed with status: {response.StatusCode}");
            return null;
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ChatResponse>(jsonResponse);
        var filterJson = result.choices[0].message.content.Trim();

        // Проверка на наличие вложенных кавычек и удаление их
        if (filterJson.StartsWith("\"") && filterJson.EndsWith("\""))
        {
            filterJson = filterJson.Substring(1, filterJson.Length - 2);
        }

        // Если строка в формате JSON массива, десериализуем как массив
        try
        {
            var filterResult = JsonSerializer.Deserialize<List<string>>(filterJson);
            if (filterResult == null || filterResult.Count == 0)
            {
                return null;
            }

            var selectedFilter = filterResult[0];
            return filters.Contains(selectedFilter) ? selectedFilter : null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON array: {ex.Message}");
            return null;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetBestMatchingFilter: {ex.Message}");
        return null;
    }
}

        // Вспомогательный класс для десериализации ответа API
        private class ChatResponse
        {
            public List<Choice> choices { get; set; }

            public class Choice
            {
                public Message message { get; set; }
            }

            public class Message
            {
                public string content { get; set; }
            }
        }

        // Пример конечной точки для использования метода
        [HttpPost("match-filter")]
        public async Task<IActionResult> MatchFilter([FromBody] FilterMatchRequest request)
        {
            var bestFilter = await GetBestMatchingFilter(request.Filters, request.MessageText);
            return Ok(new { filter = bestFilter ?? "Uncategorized" });
        }
    }

    // Модель для запроса в MatchFilter
    public class FilterMatchRequest
    {
        public List<string> Filters { get; set; }
        public string MessageText { get; set; }
    }
}