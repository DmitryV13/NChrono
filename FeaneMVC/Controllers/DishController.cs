using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Attributes;
using WebApplication1.Interfaces;
using WebApplication1.Models.Response;
using ISession = WebApplication1.Interfaces.ISession;

namespace WebApplication1.Controllers
{
    [ServiceFilter(typeof(AdminOrModeratorModeAttribute))] // Ensure only admins or moderators can access this controller
    public class DishController : Controller
    {
        private readonly IDishes _dishes; // Service for dish operations
        private readonly ISession _session; // Service for session management

        public DishController(IDishes dishes, ISession session)
        {
            _dishes = dishes;
            _session = session;
        }

        // GET: DishController/Index
        public ActionResult Index()
        {
            IEnumerable<Dish> dishes = _dishes.GetAllDishes(); // Retrieve all dishes

            if (dishes == null || !dishes.Any())
            {
                ViewBag.Message = "No dishes available. Please add some dishes."; // Display message if no dishes are available
                return View(new List<Dish>()); // Return empty list to the view
            }

            return View(dishes); // Return the list of dishes to the view
        }

        // GET: DishController/Details/5
        public ActionResult Details(Guid id)
        {
            var dishResponse = _dishes.GetDishById(id); // Retrieve dish by ID
            if (dishResponse == null || dishResponse.Dish == null)
            {
                TempData["Error"] = "Dish not found."; // Set error message if dish not found
                return RedirectToAction("Index"); // Redirect to the index page
            }
            return View(dishResponse.Dish); // Return the dish details to the view
        }

        // GET: DishController/Create
        public ActionResult AddDish()
        {
            // Return an empty model for creating a new dish
            return View(new Dish());
        }

        // POST: DishController/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        public async Task<IActionResult> AddDish(Dish dish, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate a unique file name
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                // Save the file on the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                dish.ImageUrl = $"/Images/{fileName}"; // Set the URL for the image
            }

            var dishResponse = _dishes.AddDish(dish); // Add the new dish
            if (dishResponse.Status)
            {
                TempData["Message"] = "Dish added successfully!"; // Success message
                return RedirectToAction("Index"); // Redirect to the index page
            }
            else
            {
                ModelState.AddModelError("", "Failed to add the dish. Please try again."); // Error message
            }

            return View(dish); // Return the view with the dish model
        }

        // GET: DishController/Edit/5
        public ActionResult EditDish(Guid id)
        {
            var dishResponse = _dishes.GetDishById(id); // Retrieve dish by ID
            if (dishResponse == null || dishResponse.Dish == null)
            {
                TempData["Error"] = "Dish not found."; // Set error message if dish not found
                return RedirectToAction("Index"); // Redirect to the index page
            }
            return View(dishResponse.Dish); // Return the dish to the view for editing
        }

        // POST: DishController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        public async Task<IActionResult> EditDish(Dish dish, IFormFile imageFile)
        {
            try
            {
                // Update the image if a new file is provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Generate a unique file name
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                    // Save the file on the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Update the image URL
                    dish.ImageUrl = $"/Images/{fileName}";
                }

                var dishResponse = _dishes.UpdateDish(dish.Id, dish); // Update the dish
                if (dishResponse.Status)
                {
                    TempData["Message"] = "Dish updated successfully!"; // Success message
                    return RedirectToAction("Index"); // Redirect to the index page
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update the dish. Please try again."); // Error message
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error updating dish: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the dish."); // Error message
            }

            return View(dish); // Return the view with the dish model
        }

        /*        // GET: DishController/Delete/5
                public ActionResult DeleteDish(Guid id)
                {
                    var dishResponse = _dishes.GetDishById(id); // Retrieve dish by ID
                    if (dishResponse == null || dishResponse.Dish == null)
                    {
                        TempData["Error"] = "Dish not found."; // Set error message if dish not found
                        return RedirectToAction("Index"); // Redirect to the index page
                    }
                    return View(dishResponse.Dish); // Return the dish to the view for deletion
                }*/

        [HttpPost]
        public async Task<IActionResult> DeleteDish(Guid id)
        {
            var dishResponse = await _dishes.DeleteDishAsync(id);
            if (dishResponse.Status)
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }
        }

    }
}
