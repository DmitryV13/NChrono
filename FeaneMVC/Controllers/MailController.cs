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

            string email = "wonderful_by@bk.ru";
            string password = "3BSi4CrDCQYkmyAZCB6G";
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

            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_chatBaseUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _chatApiKey);
                var prompt = $"Проанализируй текст: '{messageText}'. " +
               $"Выбери один наиболее подходящий фильтр из списка: {string.Join(", ", filters)}. " +
               $"Оцени текст по наличию ключевых слов и общему контексту, чтобы выбрать фильтр, который лучше всего отражает суть сообщения. " +
               $"Верни только JSON-массив с одним элементом – названием фильтра, например [\"filter\"].";

                var payload = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                new { role = "system", content = "Верни только JSON-массив с одним элементом, например [\"filter\"], без пояснений и лишнего текста." },
                new { role = "user", content = prompt }
            },
                    max_tokens = 30, // Увеличиваем, чтобы вместить длинные имена фильтров
                    temperature = 0.3 // Уменьшаем для более строгого следования инструкциям
                };

                var response = await client.PostAsync("chat/completions",
                    new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] Raw API response: {jsonResponse}"); // Полный ответ для диагностики

                var result = JsonSerializer.Deserialize<ChatResponse>(jsonResponse);
                if (result?.choices?.Length > 0)
                {
                    string filterContent = result.choices[0].message.content.Trim();
                    Console.WriteLine($"[DEBUG] Filter content: {filterContent}"); // Содержимое сообщения

                    // Проверяем, является ли ответ валидным JSON-массивом
                    if (filterContent.StartsWith("[") && filterContent.EndsWith("]"))
                    {
                        try
                        {
                            var filterArray = JsonSerializer.Deserialize<string[]>(filterContent);
                            if (filterArray?.Length > 0)
                            {
                                var selectedFilter = filterArray[0];
                                return filters.Contains(selectedFilter) ? selectedFilter : null;
                            }
                        }
                        catch (JsonException jsonEx)
                        {
                            Console.WriteLine($"[ERROR] Failed to parse filter content as JSON array: {jsonEx.Message}");
                        }
                    }

                    // Обработка невалидного JSON (например, текст в кавычках или с обратными кавычками)
                    var cleanedFilter = filterContent.Trim('`', '"', '[', ']', ' ', '\n');
                    if (!string.IsNullOrEmpty(cleanedFilter) && filters.Contains(cleanedFilter))
                    {
                        Console.WriteLine($"[INFO] Fallback to cleaned filter: {cleanedFilter}");
                        return cleanedFilter;
                    }

                    Console.WriteLine("[WARNING] No valid filter found in response");
                    return null;
                }

                Console.WriteLine("[WARNING] No choices in API response");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetBestMatchingFilter failed: {ex.Message}");
                return null;
            }
        }
        // Вспомогательный класс для десериализации ответа API
        public class ChatResponse
        {
            public ChatChoice[] choices { get; set; }
        }

        public class ChatChoice
        {
            public ChatMessage message { get; set; }
        }

        public class ChatMessage
        {
            public string content { get; set; }
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