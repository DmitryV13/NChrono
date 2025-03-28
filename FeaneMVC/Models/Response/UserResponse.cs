namespace WebApplication1.Models.Response
{
    public class UserResponse
    {
        public bool Status { get;set; }
        public string? Message { get;set; }
        public UserData? User { get;set; }
        public DeliveryAddress? DeliveryAddress { get; set; } 
        public bool AdminMod { get; internal set; }
        public bool ModeratorMod { get; internal set; }
    }
}
