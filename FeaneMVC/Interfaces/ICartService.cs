using System;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public interface ICartService
    {
        // Asynchronously adds an item to the cart for a specified user
        Task AddItemToCartAsync(Guid userId, CartItem item);

        // Asynchronously removes an item from the cart based on the user's ID and the dish ID
        Task RemoveItemFromCartAsync(Guid userId, Guid dishId);

        // Asynchronously updates the quantity of a specific item in the cart
        Task UpdateItemQuantityAsync(Guid userId, Guid dishId, int quantity);

        // Asynchronously retrieves the current cart for a specified user
        Task<Cart> GetCartAsync(Guid userId);

        // Asynchronously calculates the total cost of the items in the cart for a specified user
        Task<decimal> CalculateTotalAsync(Guid userId);
    }
}
