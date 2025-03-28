namespace WebApplication1.Models
{
    public class DishDto
    {
        public Guid DishId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}
