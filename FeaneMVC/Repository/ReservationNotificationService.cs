using FinalProject.Models;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication1.Interfaces;

public class ReservationNotificationService : IReservationNotifier
{
    private readonly string _smtpServer;
    private readonly string _fromEmail;

    public ReservationNotificationService(string smtpServer, string fromEmail)
    {
        _smtpServer = smtpServer;
        _fromEmail = fromEmail;
    }

    public void SendReservationConfirmation(Reservation reservation)
    {
        var subject = "Reservation Confirmation";
        var body = $"Your reservation for {reservation.ReservationDate} has been confirmed.";
        SendEmail(reservation.UserEmail, subject, body);
    }

    public void SendReservationCancellation(Reservation reservation)
    {
        var subject = "Reservation Cancellation";
        var body = $"Your reservation for {reservation.ReservationDate} has been canceled.";
        SendEmail(reservation.UserEmail, subject, body);
    }

    private void SendEmail(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_smtpServer);
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail),
            Subject = subject,
            Body = body
        };
        mailMessage.To.Add(toEmail);

        try
        {
            client.Send(mailMessage);
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Error sending email: {ex.Message}");
            // Можно выбросить исключение или обработать его по-другому
        }
    }
}
