using Microsoft.AspNetCore.Mvc;
using FinalProject.Models;
using System;
using System.Linq;
using FinalProject.Extensions;
using WebApplication1.Models.Response;
using WebApplication1.Controllers;
using System.Collections.Generic;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Factory;
using FinalProject.DbModel;
using WebApplication1.Models.Enums;
using System.Text.Json;

namespace FinalProject.Controllers
{
    public class CartController : Controller
    {

        private readonly WebApplication1.Interfaces.ISession _sessionService;
        private readonly IUSer _user;
        private readonly ApplicationDbContext _dbContext;

        // Constructor initializing dependencies and calling base constructor
        public CartController(WebApplication1.Interfaces.ISession sessionService, IUSer user, ApplicationDbContext dbContext)
            
        {
            _dbContext = dbContext;
            _sessionService = sessionService;
            _user = user;
        }

        // POST: Cart/Add
        [HttpPost]
        public async Task<IActionResult> Add(Guid dishId, string dishName, decimal dishPrice, int quantity)
        {
            // Validate input data
            if (dishId == Guid.Empty || string.IsNullOrWhiteSpace(dishName) || dishPrice <= 0 || quantity <= 0)
            {
                return Json(new { success = false, message = "Invalid input data." });
            }

            // Create new cart item
            var cartItem = new CartItem
            {
                Name = dishName,
                Price = dishPrice,
                DishId = dishId,
                Quantity = quantity,
                TotalPrice = dishPrice * quantity
            };

            // Create new cart with the item
            var cart = new Cart
            {
                CartItems = new List<CartItem> { cartItem }
            };

            // Ensure cart contains items
            if (cart.CartItems == null || !cart.CartItems.Any())
            {
                return Json(new { success = false, message = "Invalid cart data." });
            }

            try
            {
                // Retrieve user ID from session
                var userID = _sessionService.GetUserId();
                if (userID == Guid.Empty)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                // Retrieve user from database
                var user = await _user.GetOneUserByIdAsync(userID);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                // Set user ID for the cart item
                cartItem.UserId = userID;

                // Create appropriate cart based on user role
                CartFactory cartFactory = user.User.Roles == Role.VIP
                    ? new VipFactoryCart(_dbContext)
                    : new RegularUserCart(_dbContext);

                var userCart = cartFactory.CreateCart();

                // Add item to the cart
                await userCart.AddItemToCartAsync(userID, cartItem);

                // Retrieve updated cart
                var updatedCart = await userCart.GetCartAsync(userID);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log error and return failure response
                Console.Error.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while adding items to the cart." });
            }
        }

        // GET: Cart
        public async Task<IActionResult> Cart()
        {
            try
            {
                // Retrieve user ID from session
                var userID = _sessionService.GetUserId();
                if (userID == Guid.Empty)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Retrieve user from database
                var user = await _user.GetOneUserByIdAsync(userID);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Create appropriate cart based on user role
                CartFactory cartFactory = user.User.Roles == Role.VIP
                    ? new VipFactoryCart(_dbContext)
                    : new RegularUserCart(_dbContext);

                var cart = cartFactory.CreateCart();
                var userCart = await cart.GetCartAsync(userID);

                return View(userCart);
            }
            catch (Exception ex)
            {
                // Log error and return error view
                Console.Error.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Error404", "Error");
            }
        }

        // POST: Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(Guid dishId)
        {
            // Retrieve user ID from session
            var userID = _sessionService.GetUserId();
            if (userID == Guid.Empty)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve user from database
            UserResponse user = await _user.GetOneUserByIdAsync(userID);

            // Create appropriate cart based on user role
            CartFactory cartFactory = user.User.Roles == Role.VIP
                ? new VipFactoryCart(_dbContext)
                : new RegularUserCart(_dbContext);

            var cart = cartFactory.CreateCart();

            // Remove item from cart
            await cart.RemoveItemFromCartAsync(userID, dishId);

            return RedirectToAction("Cart");
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(Guid dishId, int quantity)
        {
            // Retrieve user ID from session
            var userID = _sessionService.GetUserId();
            if (userID == Guid.Empty)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve user from database
            UserResponse user = await _user.GetOneUserByIdAsync(userID);

            // Create appropriate cart based on user role
            CartFactory cartFactory = user.User.Roles == Role.VIP
                ? new VipFactoryCart(_dbContext)
                : new RegularUserCart(_dbContext);

            var cart = cartFactory.CreateCart();

            // Update item quantity in cart
            await cart.UpdateItemQuantityAsync(userID, dishId, quantity);

            return RedirectToAction("Cart");
        }
    }
}
