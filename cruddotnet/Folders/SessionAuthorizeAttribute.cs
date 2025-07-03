using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var userRole = session.GetString("UserRole");

        var controller = context.RouteData.Values["controller"]?.ToString();

        // Izinkan akses tanpa login hanya ke Auth dan Home
        if (userRole == null && controller != "Auth" && controller != "Home")
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }

        base.OnActionExecuting(context);
    }
}
