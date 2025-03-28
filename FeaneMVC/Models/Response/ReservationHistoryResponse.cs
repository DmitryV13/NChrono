namespace WebApplication1.Models.Response
{
    public class ReservationHistoryResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public ReservationHistory ReservationHistory { get; set; }
    }
}