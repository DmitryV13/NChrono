using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using ISession = WebApplication1.Interfaces.ISession;

namespace WebApplication1.Attributes
{
    public class AdminModeAttribute : ActionFilterAttribute
    {
        private readonly ISession _session;

        public AdminModeAttribute(ISession session)
        {
            _session = session;
        }

        // This method is executed before the action method is called
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            // Retrieve the cookie named "X-KEY" from the HTTP request
            var apiCookie = context.HttpContext.Request.Cookies["X-KEY"];

            if (!string.IsNullOrEmpty(apiCookie))
            {
                // Call the asynchronous method to get the user profile by cookie
                var profile =  _session.GetUserByCookie(apiCookie);

                if (profile != null)
                {
                    // Check if the user role is Admin
                    if (profile.Roles == Models.Enums.Role.Admin)
                    {
                        // Set the user profile in the current HttpContext
                        context.HttpContext.Items["UserProfile"] = profile;
                    }
                    else
                    {
                        // Redirect to error page if the user is not an Admin
                        context.Result = new RedirectToActionResult("Error404", "Error", null);
                    }
                }
                else
                {
                    // Redirect to error page if the profile is not found
                    context.Result = new RedirectToActionResult("Error404", "Error", null);
                }
            }
            else
            {
                // Redirect to error page if the cookie is not found
                context.Result = new RedirectToActionResult("Error404", "Error", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
