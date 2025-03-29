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
            List<WebApplication1.Controllers.NotificationFolder> list = new List<WebApplication1.Controllers.NotificationFolder>();
            var a = new WebApplication1.Controllers.NotificationFolder();
            var b = new List<Notification>();
            var c  = new Notification();
            c.Body = "dfghnngfds";
            c.AppName = "jhgfvg";
            c.Summary = "jhgvb";
            c.Prefix = "jhgfh";
            b.Add(c);
            a.Name = "Work";
            a.Notifications = b;
            
            list.Add(a);
            return View(list);
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