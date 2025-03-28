using FinalProject.Models;
using WebApplication1.Models;
using WebApplication1.Models.Response;

namespace WebApplication1.Interfaces
{
    public interface IReservationPaymentProcessor
    {
        // Processes a payment for a reservation using the provided payment details
        PaymentResponse ProcessPayment(Reservation reservation, PaymentDetails paymentDetails);

        // Refunds a payment for a reservation identified by its reservation ID
        PaymentResponse RefundPayment(Guid reservationId);
    }
}
