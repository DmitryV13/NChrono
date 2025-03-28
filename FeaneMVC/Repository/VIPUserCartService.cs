using FinalProject.DbModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class VIPUserCartService : ICartService
    {
        private readonly ApplicationDbContext _dbContext;

        public VIPUserCartService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Adds an item to the cart for a VIP user and applies a discount
        public async Task AddItemToCartAsync(Guid userId, CartItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }

            var cart = await GetCartAsync(userId);
            item.Price = ApplyVIPDiscount(item.Price); // Apply VIP discount
            cart.CartItems.Add(item);
            await _dbContext.SaveChangesAsync();
        }

        // Removes an item from the cart
        public async Task RemoveItemFromCartAsync(Guid userId, Guid dishId)
        {
            if (dishId == Guid.Empty)
            {
                throw new ArgumentException("Invalid dish ID", nameof(dishId));
            }

            var cart = await GetCartAsync(userId);
            var itemToRemove = cart.CartItems.FirstOrDefault(i => i.DishId == dishId);
            if (itemToRemove != null)
            {
                cart.CartItems.Remove(itemToRemove);
                await _dbContext.SaveChangesAsync();
            }
        }

        // Updates the quantity of an item in the cart and recalculates the total price
        public async Task UpdateItemQuantityAsync(Guid userId, Guid dishId, int quantity)
        {
            if (quantity < 1)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            }

            var cart = await GetCartAsync(userId);
            var itemToUpdate = cart.CartItems.FirstOrDefault(i => i.DishId == dishId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
                itemToUpdate.TotalPrice = ApplyVIPDiscount(itemToUpdate.Price) * quantity; // Recalculate total price
                await _dbContext.SaveChangesAsync();
            }
        }

        // Retrieves the cart for a user, creates a new cart if it doesn't exist
        public async Task<Cart> GetCartAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid user ID", nameof(userId));
            }

            var cart = await _dbContext.Cart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Create a new cart if one does not exist
                cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                _dbContext.Cart.Add(cart);
                await _dbContext.SaveChangesAsync();
            }

            return cart;
        }

        // Calculates the total price of all items in the cart
        public async Task<decimal> CalculateTotalAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Invalid user ID", nameof(userId));
            }

            var cart = await GetCartAsync(userId);
            return cart.CartItems.Sum(i => i.TotalPrice);
        }

        // Applies a VIP discount to the item price
        private static decimal ApplyVIPDiscount(decimal price)
        {
            // Example VIP discount (10% discount)
            return price * 0.9m;
        }
    }
}
