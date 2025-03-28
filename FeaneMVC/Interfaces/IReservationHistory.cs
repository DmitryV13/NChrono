using WebApplication1.Models;
using WebApplication1.Models.Response;

namespace WebApplication1.Interfaces
{
    public interface IReservationHistory
    {
        // Retrieves reservation history for a specific user by their identifier
        IEnumerable<ReservationHistory> GetReservationHistoryByUserId(Guid userId);

        // Retrieves reservation history for a specific item by its identifier
        IEnumerable<ReservationHistory> GetReservationHistoryByItemId(Guid itemId);

        // Adds a new reservation history entry
        ReservationHistoryResponse AddReservationHistory(ReservationHistory history);
    }
}
