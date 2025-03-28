using WebApplication1.Models;
using WebApplication1.Models.Response;

public interface IPaymentGateway
{
    // Processes a payment based on user ID and payment details
    PaymentResponse ProcessPayment(Guid userId, PaymentDetails paymentDetails);

    // Processes a refund for a specific transaction
    bool ProcessRefund(string transactionId);
}
