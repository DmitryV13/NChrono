using FinalProject.Models;

namespace WebApplication1.Models.Response
{
    public class DishResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public Dish Dish { get; set; }
    }
}
