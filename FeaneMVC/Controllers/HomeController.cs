using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FinalProject.Models;
using WebApplication1.Models.Response;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FeaneMVC.Models;
using FinalProject.DbModel;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Logger for logging information and errors
        private readonly WebApplication1.Interfaces.ISession _sessionService; // Service for session management
        private readonly IHttpContextAccessor _httpContextAccessor; // Accessor for HTTP context
        private readonly ICartService _cartService; // Service for cart operations
        private readonly IUSer _user; // Service for user operations
        private readonly ApplicationDbContext _dbContext; // Database context

        public HomeController( WebApplication1.Interfaces.ISession sessionService, IHttpContextAccessor httpContextAccessor, ICartService cartService, IUSer user, ApplicationDbContext dbContext)
        { 

            _dbContext = dbContext;
            _sessionService = sessionService;
            _httpContextAccessor = httpContextAccessor;
            _cartService = cartService;
            _user = user;
        }
        
        static async Task RunNReaderWithSchedule()
        {
            while (true)
            {
                var nots = NotificationReader();
                
                
                // CALL YOUR AI PROCESSING
                
                await Task.Delay(TimeSpan.FromMinutes(1)); 
            }
        }
        
        static List<Notification> NotificationReader()
        {
            string path = "bscripts/notifications.log"; // Укажите путь к вашему файлу
            List<Notification> vector = new List<Notification>();

            // Чтение файла
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                string accumulatedLog = ""; // Для накопления строк, относящихся к одному уведомлению

                while ((line = sr.ReadLine()) != null)
                {
                // Проверяем, начинается ли строка с временной метки (нового уведомления)
                    if (line.StartsWith("["))
                    {
                    // Если накопленный лог не пуст, парсим предыдущий и выводим
                        if (!string.IsNullOrEmpty(accumulatedLog))
                        {
                            vector.Add(ParseNotification(accumulatedLog));
                        }
                        // Начинаем новый накопленный лог
                        accumulatedLog = line;
                    }
                    else
                    {
                        // Если строка не начинается с временной метки, продолжаем накапливать ее
                        accumulatedLog += "\n" + line;
                    }
                }

                // Обработка последнего накопленного лога (если файл не пустой)
                if (!string.IsNullOrEmpty(accumulatedLog))
                {
                    vector.Add(ParseNotification(accumulatedLog));
                }
            }
            vector = vector.Distinct().ToList();
            
            // Очистка файла после обработки
            System.IO.File.WriteAllText(path, string.Empty);
            
            return vector;
        }
        
        static Notification ParseNotification(string input)
        {
            // Обновленное регулярное выражение для учета новых строк в Body и MainBody
            var regex = new Regex(@"\[([^\]]+)\] AppName - (.*?) Summary - (.*?) Body - (.*?) MainBody - (.*)", RegexOptions.Singleline);

            var match = regex.Match(input);
            if (match.Success)
            {
                return new Notification
                {
                    TimeString = match.Groups[1].Value,
                    AppName = match.Groups[2].Value,
                    Summary = match.Groups[3].Value,
                    Prefix = match.Groups[4].Value,
                    Body = match.Groups[5].Value // Теперь MainBody захватывает все оставшееся содержимое
                };
            }

            return null; // или выбросить исключение, если не удалось распарсить строку
        }


        // GET: Home/Index
        public async Task<IActionResult> Index()
        {

            RunNReaderWithSchedule();
            
            // Since GetUserId() updates the session, we need to retrieve the value again
            Guid userId = _sessionService.GetUserId();




            UserResponse user = null;
            user = await _user.GetOneUserByIdAsync(userId); // Asynchronously get user data


            // If the user is found, save user data in the session
            if (user.User != null)
            {
                _sessionService.SetSession("UserId", user.User.Id.ToString());
                _sessionService.SetSession("UserRole", user.User.Roles.ToString());
      
            }

            // Create a model to pass to the view
            CartAndDishes model;


            return View(null); // Return the view with the model
        }

        // GET: Home/About
        public IActionResult About()
        {
            return View(); // Return the view for the About page
        }

        // GET: Home/Book
        public IActionResult Book()
        {
            return View(); // Return the view for the Book page
        }

        // GET: Home/Menu
        public IActionResult Menu()
        {
            return View(); // Return the view for the Menu page
        }
    }
}
