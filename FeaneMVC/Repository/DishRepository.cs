using FinalProject.DbModel;
using FinalProject.Models;
using WebApplication1.Interfaces;
using WebApplication1.Models.Response;

namespace FeaneMVC.Repository
{
    public class DishRepository 
    {
        private readonly ApplicationDbContext _context;

        public DishRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all available dishes
     

    }
}
