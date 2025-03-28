namespace WebApplication1.Models.Response
{
    public class DeliveryResponse
    {
        public bool Success { get; set; }
        public string? Message {get;set; }
        public DeliveryAddress? DeliveryAddress { get; set; }

    }
}
