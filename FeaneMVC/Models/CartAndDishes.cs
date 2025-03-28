using FinalProject.Models;

namespace WebApplication1.Models
{
    public class CartAndDishes
    {
        public IEnumerable<Dish> dish { get; set; }
        public Cart cart { get; set; }
    }
}
