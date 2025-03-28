using FinalProject.Models;
using WebApplication1.Models.Response;

namespace WebApplication1.Interfaces
{
    // Interface for managing reservations
    public interface IReservation
    {
        // Creates a new reservation for the specified user
        ReservationResponse CreateReservation(Reservation reservation, Guid userId);

        // Cancels an existing reservation by its identifier
        ReservationResponse CancelReservation(Guid reservationId);

        // Retrieves a reservation by its identifier
        ReservationResponse GetReservationById(Guid reservationId);

        // Retrieves all reservations
        IEnumerable<Reservation> GetAllReservations();

        // Retrieves reservations for a specific user
        IEnumerable<Reservation> GetReservationsByUserId(Guid userId);

        // Updates an existing reservation by its identifier
        ReservationResponse UpdateReservation(Guid reservationId, Reservation reservation);
    }
}
