
namespace WebApplication1.Models
{
    public class Authentification
    {
        public string Password { get;  set; }
        public Guid Id { get;  set; }
        public string Email { get;  set; }
        public string Username { get;  set; }
        public string Credential { get;  set; }
        public bool RememberMe { get; internal set; }
    }
}