using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models.Response;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using System.Collections.Generic;
using FinalProject.DbModel;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Logger for logging information and errors
        private readonly WebApplication1.Interfaces.ISession _sessionService; // Service for session management
        private readonly IHttpContextAccessor _httpContextAccessor; // Accessor for HTTP context
        private readonly IUSer _user; // Service for user operations
        private readonly ApplicationDbContext _dbContext; // Database context

        public HomeController(WebApplication1.Interfaces.ISession sessionService, IHttpContextAccessor httpContextAccessor,IUSer user, ApplicationDbContext dbContext)
        { 
            _dbContext = dbContext;
            _sessionService = sessionService;
            _httpContextAccessor = httpContextAccessor;
            _user = user;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {

            // Since GetUserId() updates the session, we need to retrieve the value again
            Guid userId = _sessionService.GetUserId();



            // Retrieve all dishes

            UserResponse user = null;
            user = await _user.GetOneUserByIdAsync(userId); // Asynchronously get user data


            // If the user is found, save user data in the session
            if (user.User != null)
            {
                _sessionService.SetSession("UserId", user.User.Id.ToString());
                _sessionService.SetSession("UserRole", user.User.Roles.ToString());
      
            }


            return View(); // Return the view with the model
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
