using FeaneMVC.Models;
using FinalProject.DbModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;

namespace FeaneMVC.Controllers
{
    [ApiController]
    [Route("api/filters")]
    public class FiltersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly WebApplication1.Interfaces.ISession _sessionService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _chatApiKey;
        private readonly string _chatBaseUrl;

        public FiltersController(
            ApplicationDbContext context,
            WebApplication1.Interfaces.ISession sessionService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _chatApiKey = configuration["ChatApi:Key"] ?? throw new ArgumentException("Chat API key is not configured.");
            _chatBaseUrl = configuration["ChatApi:BaseUrl"] ?? throw new ArgumentException("Chat API base URL is not configured.");
        }

        [HttpPost]
        public async Task<IActionResult> AddFilters([FromBody] List<string> userInput)
        {
            if (userInput == null || !userInput.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                return BadRequest("No valid input provided.");
            }

            var userId = _sessionService.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID could not be retrieved.");
            }

            // Извлечение фильтров с помощью AI
            var filters = await GetFiltersFromAI(string.Join(" ", userInput));
            if (filters == null || !filters.Any())
            {
                return BadRequest("Failed to generate filters from input.");
            }

            // Сохранение фильтров в БД
            foreach (var filter in filters)
            {
                var userFilter = new UserFilter
                {
                    Id = Guid.NewGuid(),
                    Filter = filter,
                    UserId = userId
                };
                _context.UserFilters.Add(userFilter);
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Filters saved.", Filters = filters });
        }

        [HttpGet]
        [Route("~/Filters/Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("filter-page")]
public IActionResult FilterPage()
{
    var folders = new List<NotificationFolder>
    {
        new NotificationFolder
        {
            Name = "Work",
            Notifications = new List<Notification>
            {
                new Notification
                {
                    Body = "Уведомление о собрании в 14:00.",
                    AppName = "Microsoft Teams",
                    Summary = "Запланированное собрание",
                    Prefix = "Работа",
                    TimeString = DateTime.Now.AddMinutes(-30).ToString("g")
                },
                new Notification
                {
                    Body = "Документ отчета успешно загружен на сервер.",
                    AppName = "OneDrive",
                    Summary = "Загрузка завершена",
                    Prefix = "Работа",
                    TimeString = DateTime.Now.AddHours(-1).ToString("g")
                },
                new Notification
                {
                    Body = "Задача 'Подготовка отчета' завершена.",
                    AppName = "JIRA",
                    Summary = "Завершение задачи",
                    Prefix = "Работа",
                    TimeString = DateTime.Now.AddDays(-1).ToString("g")
                },
                new Notification
                {
                    Body = "Ваши рабочие часы на этой неделе: 40 часов.",
                    AppName = "Time Tracker",
                    Summary = "Отчет о времени",
                    Prefix = "Работа",
                    TimeString = DateTime.Now.AddDays(-2).ToString("g")
                }
            }
        },
        new NotificationFolder
        {
            Name = "Personal",
            Notifications = new List<Notification>
            {
                new Notification
                {
                    Body = "Подарок доставлен на пункт выдачи.",
                    AppName = "Ozon",
                    Summary = "Доставка выполнена",
                    Prefix = "Личное",
                    TimeString = DateTime.Now.AddDays(-1).ToString("g")
                },
                new Notification
                {
                    Body = "Сообщение от друга: \"Как дела?\"",
                    AppName = "Telegram",
                    Summary = "Новое сообщение",
                    Prefix = "Личное",
                    TimeString = DateTime.Now.AddMinutes(-5).ToString("g")
                },
                new Notification
                {
                    Body = "Напоминание: День рождения Марии завтра.",
                    AppName = "Google Calendar",
                    Summary = "Напоминание",
                    Prefix = "Личное",
                    TimeString = DateTime.Now.AddHours(-12).ToString("g")
                }
            }
        },
        new NotificationFolder
        {
            Name = "Social",
            Notifications = new List<Notification>
            {
                new Notification
                {
                    Body = "Ваш пост в группе получил 50 лайков.",
                    AppName = "VK",
                    Summary = "Повышенная активность",
                    Prefix = "Соцсети",
                    TimeString = DateTime.Now.AddHours(-3).ToString("g")
                },
                new Notification
                {
                    Body = "Пользователь @john_doe оставил комментарий на вашу фотографию.",
                    AppName = "Instagram",
                    Summary = "Новый комментарий",
                    Prefix = "Соцсети",
                    TimeString = DateTime.Now.AddMinutes(-15).ToString("g")
                },
                new Notification
                {
                    Body = "Вас добавили в новый чат: 'Планируем отпуск'.",
                    AppName = "WhatsApp",
                    Summary = "Новый чат",
                    Prefix = "Соцсети",
                    TimeString = DateTime.Now.AddHours(-5).ToString("g")
                }
            }
        },
        new NotificationFolder
        {
            Name = "System",
            Notifications = new List<Notification>
            {
                new Notification
                {
                    Body = "Успешная установка обновлений безопасности.",
                    AppName = "Windows Update",
                    Summary = "Системное обновление",
                    Prefix = "Система",
                    TimeString = DateTime.Now.AddDays(-2).ToString("g")
                }
            }
        },
        new NotificationFolder
        {
            Name = "Finance",
            Notifications = new List<Notification>
            {
                new Notification
                {
                    Body = "Поступление заработной платы на карту: 45,000 руб.",
                    AppName = "Сбербанк Онлайн",
                    Summary = "Поступление средств",
                    Prefix = "Финансы",
                    TimeString = DateTime.Now.AddHours(-8).ToString("g")
                },
                new Notification
                {
                    Body = "Списание на сумму 1,200 руб. за покупку в магазине 'М.Видео'.",
                    AppName = "Тинькофф",
                    Summary = "Транзакция",
                    Prefix = "Финансы",
                    TimeString = DateTime.Now.AddMinutes(-50).ToString("g")
                },
                new Notification
                {
                    Body = "Оплата мобильной связи на сумму 300 руб.",
                    AppName = "МТС Банк",
                    Summary = "Пополнение счета",
                    Prefix = "Финансы",
                    TimeString = DateTime.Now.AddDays(-1).ToString("g")
                },
                new Notification
                {
                    Body = "Налоговая декларация успешно подана.",
                    AppName = "Госуслуги",
                    Summary = "Подтверждение подачи",
                    Prefix = "Финансы",
                    TimeString = DateTime.Now.AddHours(-18).ToString("g")
                }
            }
        }
    };

    return View(folders);
}



        [HttpGet("all")]
        public IActionResult GetUserFilters()
        {
            var userId = _sessionService.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID could not be retrieved.");
            }

            var userFilters = _context.UserFilters
                .Where(f => f.UserId == userId)
                .Select(f => f.Filter)
                .Distinct()
                .ToList();

            var filters = new Dictionary<string, List<string>>();
            foreach (var filter in userFilters)
            {
                filters.Add(filter, new List<string> { "sdfsdfdsf", "sdfdsfsdfsdf", "sdfsdfsdf" });
            }

            return Ok(filters);
        }

        [HttpGet("add-mail")]
        public IActionResult AddMail()
        {
            return View();
        }

        [HttpPost("add-mail")]
        public IActionResult AddMail([FromForm] string email, [FromForm] string code)
        {
            var userId = _sessionService.GetUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                var mailCheck = new MailCheck
                {
                    Email = email,
                    Code = code,
                    UserId = userId
                };
                _context.MailChecks.Add(mailCheck);
                _context.SaveChanges();
            }
            return Ok();
        }

        private async Task<List<string>> GetFiltersFromAI(string userInput)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _chatApiKey);

            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Твоя задача — проанализировать текст пользователя и извлечь из него ровно одно ключевое слово, которое лучше всего подходит для фильтра. Верни результат в виде JSON-массива, содержащего только одну строку. Исключай лишние слова и выбирай наиболее релевантное понятие. Пример: Ввод 'Я люблю бегать по утрам' → [\"бег\"]. Не добавляй пояснений, только массив с одним словом." },
                    new { role = "user", content = userInput }
                },
                max_tokens = 50
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{_chatBaseUrl}chat/completions", content);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API request failed with status: {response.StatusCode}");
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ChatResponse>(jsonResponse);
                var filtersJson = result.choices[0].message.content.Trim();

                if (!filtersJson.StartsWith("[") || !filtersJson.EndsWith("]"))
                {
                    Console.WriteLine("Invalid JSON array format: " + filtersJson);
                    return null;
                }

                return JsonSerializer.Deserialize<List<string>>(filtersJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetFiltersFromAI: {ex.Message}");
                return null;
            }
        }
    }

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
    
    
    public class NotificationFolder
    {
        public string Name { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}