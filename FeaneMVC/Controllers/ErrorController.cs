using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class ErrorController:Controller
    {
        public IActionResult Error404()
        {
            return View();
        }
    }
}
