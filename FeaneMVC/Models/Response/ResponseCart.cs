using FinalProject.Models;

namespace WebApplication1.Models.Response
{
    public class ResponseCart
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public Cart? Cart { get; set; }
    }
}
