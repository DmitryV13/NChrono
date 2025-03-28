using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Models.Response;
using ISession = WebApplication1.Interfaces.ISession;

namespace WebApplication1.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IReservation _reservation;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;

        public ReservationController(IReservation reservation, IHttpContextAccessor httpContextAccessor,ISession session)
        {
            _session = session;
            _reservation = reservation;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Reservation/Book
        public IActionResult Book()
        {

            // Retrieve the user ID from the session
            Guid userId = _session.GetUserId();

            // Check if the user ID is valid (not empty or Guid.Empty)
            if (userId == Guid.Empty)
            {
                // If the user ID is invalid, redirect to the authentication page
                return RedirectToAction("Authentication");
            }

            // User ID is valid, return the booking view
            return View();
            
           
        }

        // POST: Reservation/ReservationProcess
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReservationProcess(Reservation data)
        {
            // Retrieve the user ID from the session
            Guid userId =_session.GetUserId();

            // Check if the user ID is valid (not empty or Guid.Empty)
            if (userId == Guid.Empty)
            {
                // If the user ID is invalid, redirect to the authentication page
                return RedirectToAction("Authentication");
            }


            // Create the reservation using the validated user ID
            ReservationResponse reservationResponse = _reservation.CreateReservation(data, userId);

            // Check the response status
            if (reservationResponse.Status)
            {
                // Reservation created successfully, redirect to the home page
                return RedirectToAction("Index", "Home");
            }

            // Reservation failed, display the error message
            ViewBag.ErrorMessage = reservationResponse.Message;
            return RedirectToAction("Error404", "Error");
        }
    }
}
