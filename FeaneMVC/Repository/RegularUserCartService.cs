using FinalProject.DbModel;
using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RegularUserCartService : ICartService
{
    private readonly ApplicationDbContext _dbContext;

    public RegularUserCartService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddItemToCartAsync(Guid userId, CartItem item)
    {
        var cart = await GetCartAsync(userId);
        if (cart != null)
        {
            // Set the CartId for the item and add it to the cart
            item.CartId = cart.CartId;
            item.Cart = cart;
            cart.CartItems.Add(item);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveItemFromCartAsync(Guid userId, Guid dishId)
    {
        var cart = await GetCartAsync(userId);
        if (cart != null)
        {
            // Find the item to remove and remove it from the cart
            var itemToRemove = cart.CartItems.FirstOrDefault(i => i.DishId == dishId);
            if (itemToRemove != null)
            {
                cart.CartItems.Remove(itemToRemove);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    public async Task UpdateItemQuantityAsync(Guid userId, Guid dishId, int quantity)
    {
        try
        {
            var cart = await GetCartAsync(userId);
            if (cart != null)
            {
                // Find the item to update and set the new quantity
                var itemToUpdate = cart.CartItems.FirstOrDefault(i => i.DishId == dishId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.Quantity = quantity;
                    itemToUpdate.TotalPrice = itemToUpdate.Price * quantity;
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception (use a logging framework in a real application)
            Console.Error.WriteLine($"Error updating item quantity: {ex.Message}");
        }
    }

    public async Task<Cart> GetCartAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                // Create a new cart if the user ID is empty
                return new Cart
                {
                    CartId = Guid.NewGuid(),
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
            }

            // Retrieve the cart from the database, including its items
            var cart = await _dbContext.Cart
                                        .Include(c => c.CartItems)
                                        .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Create a new cart if none is found
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
        catch (Exception ex)
        {
            // Log the exception (use a logging framework in a real application)
            Console.Error.WriteLine($"Error retrieving cart: {ex.Message}");

            // Return a new cart as a fallback
            return new Cart
            {
                CartId = Guid.NewGuid(),
                UserId = userId,
                CartItems = new List<CartItem>()
            };
        }
    }

    public async Task<decimal> CalculateTotalAsync(Guid userId)
    {
        var cart = await GetCartAsync(userId);
        // Calculate the total price of the items in the cart
        return cart.CartItems.Sum(i => i.TotalPrice);
    }
}
