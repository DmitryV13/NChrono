using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Text;
using WebApplication1.Models;
using WebApplication1.Models.Response;

namespace WebApplication1.Interfaces
{
    public interface ISession
    {
        // Sets a user cookie with the specified login credentials and expiration settings
        void SetUserCookie(string loginCredential, bool rememberMe);

        // Retrieves user data based on the provided cookie value
        UserData GetUserByCookie(string cookieValue);

        // Logs out the user and clears any session-related data
        void UserLogout();
        public Guid GetUserId();
        public Task SessionStatus();
        public void SetSession(string name, string v);
    }
}
