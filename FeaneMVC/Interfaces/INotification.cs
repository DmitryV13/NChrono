using WebApplication1.Models.Response;

namespace WebApplication1.Interfaces
{
    public interface INotification
    {
        // Sends a notification with a message to the specified recipient via email
        void SendNotification(string message, string recipientEmail);
    }
}
