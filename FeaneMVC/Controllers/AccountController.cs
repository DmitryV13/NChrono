using FinalProject.DbModel;
using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApplication1.Controllers;
using WebApplication1.Helpers;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Models.Response;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SummerWork.WebApi.Controllers
{
    public class AccountController :Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDishes _dishes;
        private readonly WebApplication1.Interfaces.ISession _sessionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICartService _cartService;
        private readonly IUSer _user;
        private readonly IReservation _reservation;
        private readonly ApplicationDbContext _dbContext;

        // Constructor to initialize dependencies
        public AccountController(IDishes dishes, IReservation reservation, WebApplication1.Interfaces.ISession sessionService, IHttpContextAccessor httpContextAccessor, ICartService cartService, IUSer user, ApplicationDbContext dbContext)        
        {
            _reservation = reservation;
            _dishes = dishes;
            _dbContext = dbContext;
            _sessionService = sessionService;
            _httpContextAccessor = httpContextAccessor;
            _cartService = cartService;
            _user = user;
        }

        // GET: /Account/Authentication
        public IActionResult Authentication()
        {
            return View();
        }

        // GET: /Account/Profile
        public async Task<IActionResult> Profile()
        {
            // Retrieve the user ID
            Guid userId = _sessionService.GetUserId();

            // Retrieve the user information by ID
            var user = await _user.GetOneUserByIdAsync(userId);

            // Check if the user was found and if their status is active
            if (user != null && user.Status)
            {
                // If the user is active, get the reservation history and pass it to the view
                var reservHistory = _reservation.GetAllReservations();
                return View(reservHistory);
            }
            else
            {
                // If the user was not found or their status is inactive, redirect to the authentication page
                return RedirectToAction("Authentication");
            }
        }



        // GET: /Account/Contacts
        public async Task<IActionResult> Contacts()
        {
            Guid userId = _sessionService.GetUserId();




            var user = await _user.GetOneUserByIdAsync(userId);

            if (user.Status)
            {
                return View(user.User);
            }
            else
            {
                return RedirectToAction("Authentication");
            }


        }

        // GET: /Account/Discounts
        public IActionResult Discounts()
        {
            return View();
        }

        // GET: /Account/Addresses
        public async Task<IActionResult> Addresses()
        {
            Guid userId = _sessionService.GetUserId();




            var addressResponse = await _user.GetOneAddressByUserIdAsync(userId);

            if (addressResponse.Success)
            {
                return View(addressResponse.DeliveryAddress);
            }
            else
            {
                return RedirectToAction("Authentication");
            }

        }

        // POST: /Account/UpdateContacts
        [HttpPost]
        public async Task<JsonResult> UpdateContacts(UserData data)
        {
            // Retrieve the user ID from the session or another method
            Guid userId = _sessionService.GetUserId();


            // Assign the valid user ID to the data object
            data.Id = userId;

            // Attempt to update the user information
            var updateResponse = await _user.UpdateUser(data);

            // If the update operation fails, return a JSON response with the error message
            if (!updateResponse.Status)
            {
                return Json(new { success = false, message = updateResponse.Message });
            }

            // If the update operation succeeds, return a JSON response with success message and updated user data
            return Json(new { success = true, message = "User updated successfully.", user = updateResponse.User });
        }


        // POST: /Account/UpdateAddress
        [HttpPost]
        public async Task<JsonResult> UpdateAddress(DeliveryAddress data)
        {
            Guid userId = _sessionService.GetUserId();


            var userResponse = await _user.GetOneUserByIdAsync(userId);

            if (!userResponse.Status)
            {
                return Json(new { success = false, message = userResponse.Message });
            }

            var updateResponse = await _user.UpdateAddress(userResponse.User, data);

            if (!updateResponse.Status)
            {
                return Json(new { success = false, message = updateResponse.Message });
            }

            return Json(new { success = true, message = "User updated successfully.", user = updateResponse.User });
        }

        // GET: /Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return RedirectToAction("ResetPassword");
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Please enter a valid email address.";
                return View();
            }

            var resetResponse = _user.ChangeUserPassword(email);

            if (resetResponse.Status)
            {
                // Logic for sending the new password to the user's email
                ViewBag.Message = $"A new password has been sent to {email}.";
            }
            else
            {
                ViewBag.Error = resetResponse.Message ?? "Failed to reset password. Please try again.";
            }

            return RedirectToAction("Authentication");
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(Authentification data)
        {
            if (data == null)
            {
                return RedirectToAction("Error404", "Error");
            }

            var registerData = new UserData()
            {
                Password = data.Password,
                Id = data.Id,
                Email = data.Email,
                Username = data.Username,
                Credential = data.Credential,
                Roles = Role.User,
                IP = HttpContext.Connection?.RemoteIpAddress?.ToString() ?? "Unknown",
                FirstRegisterTime = DateTime.Now
            };

            if (_user == null)
            {
                throw new InvalidOperationException("User service is not available.");
            }

            var registerUser = _user.AddUser(registerData);

            if (registerUser == null || !registerUser.Status)
            {
                TempData["RegisterError"] = registerUser.Message;
                return RedirectToAction("Authentication");
            }

            return RedirectToAction("Authentication");
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(Authentification data)
        {
            _sessionService.GetUserId(); // Ensure session is updated

            var userLogin = _user.AuthenticateUser(data.Credential, data.Password);

            if (userLogin == null || !userLogin.Status)
            {
                ViewBag.ErrorMessage = "Invalid credentials.";
                return RedirectToAction("Authentication");
            }

            _sessionService.SetUserCookie(data.Credential, true); // Use session service for cookie management
            _sessionService.SetSession("IsUserLoggedIn", "true");
            ViewBag.UserName = data.Credential;

            return RedirectToAction("Profile");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            _sessionService.UserLogout();
            return RedirectToAction("Index", "Home");
        }
    }
}
