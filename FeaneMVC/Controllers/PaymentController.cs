using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.Response;

namespace WebApplication1.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentGateway _payment; // Service for handling payment processing
        private readonly WebApplication1.Interfaces.ISession _sessionService;

        public PaymentController(IPaymentGateway payment, WebApplication1.Interfaces.ISession sessionService)
        {
            _payment = payment;
            _sessionService = sessionService;
        }

        // GET: Payment/Checkout
        public IActionResult Checkout(decimal amount)
        {
            // Create a PaymentDetails object with the provided amount
            PaymentDetails price = new()
            {
                TotalPrice = amount
            };

            // Pass the PaymentDetails object to the view
            return View(price);
        }

        // POST: Payment/SubmitPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitPayment(PaymentDetails paymentDetails)
        {
            // Get the user ID from the session
            Guid userId = _sessionService.GetUserId();

            // Check if the user ID is valid (not empty or Guid.Empty)
            if (userId == Guid.Empty)
            {
                // If the user ID is invalid, redirect to the authentication page
                return RedirectToAction("Authentication");
            }


            // Process the payment
            PaymentResponse paymentResponse = _payment.ProcessPayment(userId, paymentDetails);

            // Redirect to a confirmation page or handle success/error
            if (paymentResponse.Success)
            {
                return RedirectToAction("Confirmation");
            }
            else
            {
                // Handle payment failure
                ModelState.AddModelError("", "Payment processing failed. Please try again.");
                return View("Checkout", paymentDetails); // Return to checkout view with error message
            }
        }

        // GET: Payment/Confirmation
        public IActionResult Confirmation()
        {
            return View(); // Return the view for the confirmation page
        }
    
    }
}
