using WebApplication1.Interfaces;
using WebApplication1.Models.Response;
using WebApplication1.Models;
using FinalProject.DbModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication1.Models.Enums;
using Microsoft.Extensions.Logging;
using FinalProject.Models;
using WebApplication1.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FoodShop.Repository
{
    public class UserRepository : IUSer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;
        private readonly INotification _notification;
        // Constructor for UserRepository
        public UserRepository(IHttpContextAccessor httpContextAccessor, INotification notification, ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notification = notification;
        }

        // Retrieves all users with data existence check
        public IEnumerable<UserData> GetAllUsers()
        {
            try
            {
                var users = _context.Users.ToList();
                if (users == null || !users.Any())
                {
                    return new List<UserData>(); // Return an empty list if no users found
                }
                return users;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                return new List<UserData>(); // Return an empty list in case of an error
            }
        }

        // Retrieves a single user by Id with existence check
        public async Task<UserResponse> GetOneUserByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new UserResponse { Message = "Invalid ID", Status = false }; // Validate the ID
                }

                // Asynchronous search for the user
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return new UserResponse { Message = "User not found", Status = false }; // Check if the user exists
                }

                return new UserResponse { Status = true, Message = "User retrieved successfully", User = user };
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while retrieving user with ID: {id}");
                return new UserResponse { Message = "An error occurred while retrieving the user.", Status = false };
            }
        }
        // Adds a new user with checks for existence and data validity
        public UserResponse AddUser(UserData user)
        {
            try
            {
                if (user == null)
                {
                    return new UserResponse { Message = "User data is null", Status = false }; // Validate user data
                }

                // Check if a user with the same email already exists
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    return new UserResponse { Message = "User with the same email already exists", Status = false };
                }

                // Hash the user's password
                user.Password = LoginHelper.HashGen(user.Password);

                // Create a new cart for the user
                var cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    UserId = user.Id
                };

                // Create a new delivery address
                var deliveryAddress = new DeliveryAddress
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id
                };

                // Associate the user with the cart and delivery address
                user.DeliveryId = deliveryAddress.Id;
                user.CartId = cart.CartId;
                user.Delivery = deliveryAddress;
                user.Cart = cart;

                // Add the cart and delivery address to the context
                _context.Cart.Add(cart);
                _context.DeliveryAddresses.Add(deliveryAddress);
                _context.Users.Add(user);

                // Save changes to the database
                _context.SaveChanges();

                return new UserResponse { Status = true, Message = "User added successfully", User = user };
            }
            catch (Exception ex)
            {
                // Log the exception, if you have a logger
                // _logger.LogError(ex, "An error occurred while adding the user.");

                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                return new UserResponse { Message = "An error occurred while adding the user.", Status = false };
            }
        }

        // Updates the address with new data
        public async Task<UserResponse> UpdateAddress(UserData addressOld, DeliveryAddress newAddress)
        {
            try
            {
                if (newAddress == null)
                {
                    return new UserResponse { Message = "No new data provided.", Status = false }; // Validate new address data
                }

                // Find the existing delivery address by UserId
                var delivery = await _context.DeliveryAddresses
                    .SingleOrDefaultAsync(d => d.UserId == addressOld.Id);

                if (delivery == null)
                {
                    return new UserResponse { Message = "Address not found.", Status = false }; // Check if address exists
                }

                // Update address fields if provided
                if (!string.IsNullOrEmpty(newAddress.MoreInfo))
                {
                    delivery.MoreInfo = newAddress.MoreInfo;
                }

                if (!string.IsNullOrEmpty(newAddress.City))
                {
                    delivery.City = newAddress.City;
                }

                if (!string.IsNullOrEmpty(newAddress.Street))
                {
                    delivery.Street = newAddress.Street;
                }

                if (!string.IsNullOrEmpty(newAddress.Country))
                {
                    delivery.Country = newAddress.Country;
                }

                if (!string.IsNullOrEmpty(newAddress.ParcelIndex))
                {
                    delivery.ParcelIndex = newAddress.ParcelIndex;
                }

                // Update the delivery address for the user
                var user = await _context.Users.SingleOrDefaultAsync(U => U.Id == addressOld.Id);
                user.Delivery = delivery;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return new UserResponse { Message = "Address updated successfully", Status = true, DeliveryAddress = delivery };
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while updating the address.");

                return new UserResponse { Message = "An error occurred while updating the address.", Status = false };
            }
        }
        // Retrieves a delivery address by UserId
        public async Task<DeliveryResponse> GetOneAddressByUserIdAsync(Guid userId)
        {
            try
            {
                // Find the delivery address by UserId
                var delivery = await _context.DeliveryAddresses
                    .FirstOrDefaultAsync(d => d.UserId == userId);

                if (delivery == null)
                {
                    return new DeliveryResponse
                    {
                        Message = "Address not found.",
                        Success = false
                    };
                }

                return new DeliveryResponse
                {
                    Message = "Address retrieved successfully.",
                    Success = true,
                    DeliveryAddress = delivery
                };
            }
            catch (Exception ex)
            {
                // Log the exception, if you have a logger
                // _logger.LogError(ex, "An error occurred while retrieving the address.");

                return new DeliveryResponse
                {
                    Message = "An error occurred while retrieving the address.",
                    Success = false
                };
            }
        }

        // Updates a user's information
        public async Task<UserResponse> UpdateUser(UserData userNew)
        {
            try
            {
                // Find the user by ID
                var userOld = await _context.Users.FirstOrDefaultAsync(u => u.Id == userNew.Id);

                // Check if the user exists
                if (userOld == null)
                {
                    return new UserResponse { Message = "User not found.", Status = false };
                }

                // Update user details
                userOld.Username = userNew.Username;
                userOld.Email = userNew.Email;
                userOld.Address = userNew.Address;
                userOld.PhoneNumber = userNew.PhoneNumber;
                userOld.Roles = userNew.Roles;

                // Save changes
                await _context.SaveChangesAsync();

                return new UserResponse { Message = "User updated successfully", Status = true, User = userOld };
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, $"An error occurred while updating user with ID: {userNew.Id}");

                return new UserResponse { Message = "An error occurred while updating the user.", Status = false };
            }
        }

        // Deletes a user by ID
        public UserResponse DeleteUser(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new UserResponse { Message = "Invalid ID", Status = false }; // Validate ID
                }

                var user = _context.Users
                    .Include(u => u.Cart)
                    .Include(u => u.Delivery)
                    .FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    return new UserResponse { Message = "User not found", Status = false };
                }

                // Remove all records related to the user
                if (user.Cart != null)
                {
                    _context.Cart.Remove(user.Cart);
                }

                if (user.Delivery != null)
                {
                    _context.DeliveryAddresses.Remove(user.Delivery);
                }

                _context.Users.Remove(user);
                _context.SaveChanges();

                return new UserResponse { Message = "User deleted successfully", Status = true };
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, $"An error occurred while deleting user with ID: {id}");

                return new UserResponse { Message = "An error occurred while deleting the user.", Status = false };
            }
        }

        // Searches for users by name
        public IEnumerable<UserData> FindUsersByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new List<UserData>(); // Validate name
                }

                return _context.Users.Where(u => u.Username.Contains(name)).ToList();
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, $"An error occurred while searching for users with name: {name}");

                return new List<UserData>();
            }
        }

        // Authenticates a user
        public UserResponse AuthenticateUser(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return new UserResponse { Message = "Username or password is invalid", Status = false };
                }

                // Hash the password
                password = LoginHelper.HashGen(password);

                // Find the user in the database
                var user = _context.Users.SingleOrDefault(u => u.Username == username && u.Password == password);

                if (user == null)
                {
                    return new UserResponse { Message = "Authentication failed", Status = false };
                }

                // Access HttpContext via _httpContextAccessor
                var session = _httpContextAccessor.HttpContext.Session;

                // Store user ID and role in the session
                session.SetString("UserId", user.Id.ToString());
                session.SetString("UserRole", user.Roles.ToString());

                // Update last login time
                user.FirstLoginTime = DateTime.Now;
                _context.SaveChanges();

                return new UserResponse { Message = "User authenticated successfully", Status = true, User = user };
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, "An error occurred during user authentication.");

                return new UserResponse { Message = "An error occurred during authentication.", Status = false };
            }
        }

        // Retrieves user roles by user ID
        public IEnumerable<Role> GetUserRoles(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    return Enumerable.Empty<Role>(); // Validate ID
                }

                var user = _context.Users.SingleOrDefault(u => u.Id == Id);
                if (user == null)
                {
                    return Enumerable.Empty<Role>(); // Return empty if user not found
                }

                // Return a collection containing the user's role
                return new List<Role> { user.Roles };
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, $"An error occurred while retrieving roles for user with ID: {Id}");
                return Enumerable.Empty<Role>(); // Return empty on error
            }
        }

        // Changes the user's password
        public UserResponse ChangeUserPassword(string email)
        {
            try
            {



                if (string.IsNullOrWhiteSpace(email))
                {
                    return new UserResponse { Message = "Email is invalid", Status = false }; // Validate new password
                }

                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    return new UserResponse { Message = "User not found", Status = false }; // Check if user exists
                }
                var password = PasswordGenerator.GeneratePassword();
                _notification.SendNotification($"New password is:{password}",email);

                user.Password = LoginHelper.HashGen(password); // Hash the new password
                _context.SaveChanges();
                _context.SaveChanges();

                return new UserResponse { Message = "Password changed successfully", Status = true, User = user };
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, $"An error occurred while changing password for user with ID: {Id}");
                return new UserResponse { Message = "An error occurred while changing the password.", Status = false };
            }
        }

        // Checks if a user exists by ID
        public UserResponse IsUserExists(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    return new UserResponse { Message = "Invalid ID", Status = false }; // Validate ID
                }

                var exists = _context.Users.Any(u => u.Id == Id);
                return new UserResponse { Status = exists, Message = exists ? "User exists" : "User does not exist" };
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, $"An error occurred while checking if user with ID: {Id} exists.");
                return new UserResponse { Message = "An error occurred while checking the user's existence.", Status = false };
            }
        }
        // Assigns a role to a user
        public UserResponse AssignRoleToUser(Guid userId, Role role)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return new UserResponse { Message = "User not found", Status = false }; // User not found
                }

                // Check if the user already has this role
                if (user.Roles == role)
                {
                    return new UserResponse { Message = "Role already assigned to user", Status = false }; // Role already assigned
                }

                // Assign the new role
                user.Roles = role;

                _context.SaveChanges();
                return new UserResponse { Message = "Role assigned successfully", Status = true, User = user };
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, $"An error occurred while assigning role to user with ID: {userId}");
                return new UserResponse { Message = "An error occurred while assigning the role.", Status = false };
            }
        }

        // Deactivates a user
        public UserResponse DeactivateUser(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                {
                    return new UserResponse { Message = "Invalid ID", Status = false }; // Validate ID
                }

                var user = _context.Users.Find(Id);
                if (user == null)
                {
                    return new UserResponse { Message = "User not found", Status = false }; // User not found
                }

                user.IsActive = false; // Assuming IsActive property exists
                _context.SaveChanges();

                return new UserResponse { Message = "User deactivated successfully", Status = true, User = user };
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, $"An error occurred while deactivating user with ID: {Id}");
                return new UserResponse { Message = "An error occurred while deactivating the user.", Status = false };
            }
        }

        // Retrieves user data based on provided credentials
        public UserResponse GetUserData(UserData data)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == data.Credential || u.Username == data.Credential);
                if (user != null)
                {
                    user.FirstLoginTime = DateTime.Now;
                    _context.SaveChanges();

                    if (data.Password == user.Password) // Check if password matches
                    {
                        return new UserResponse() { Status = true };
                    }
                    else
                    {
                        return new UserResponse() { Status = false };
                    }
                }
                else
                {
                    return new UserResponse() { Status = false }; // User not found
                }
            }
            catch (Exception ex)
            {
                // Log the
                //
                // , if you have a logger
                // _logger.LogError(ex, $"An error occurred while retrieving user data for credential: {data.Credential}");
                return new UserResponse() { Status = false };
            }
        }

        // Retrieves user by cookie value asynchronously
        public async Task<UserData> GetUserByCookie(string cookieValue)
        {
            try
            {
                if (string.IsNullOrEmpty(cookieValue))
                {
                    return null; // Validate cookie value
                }

                var user = await _context.Users
                    .AsNoTracking() // Disable tracking for performance
                    .FirstOrDefaultAsync(u => u.CookieValue == cookieValue);

                return user;
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, $"An error occurred while retrieving user by cookie value: {cookieValue}");
                return null;
            }
        }

        // Logs out the user
        public ResponseMain UserLogout()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                // Clear session data
                httpContext.Session.Clear();

                // Remove user cookie if it exists
                if (httpContext.Request.Cookies.ContainsKey("UserCookie"))
                {
                    httpContext.Response.Cookies.Delete("UserCookie");
                }

                // Create response
                return new ResponseMain
                {
                    Status = true,
                    Message = "User successfully logged out."
                };
            }
            catch (Exception ex)
            {
                // Log the error, if you have a logger
                // _logger.LogError(ex, "An error occurred while logging out the user.");
                return new ResponseMain
                {
                    Status = false,
                    Message = "An error occurred while logging out the user."
                };
            }
        }

    }
}
