using System.Security.Claims;

namespace eStore.Helper
{


    public class RedirectAdminMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectAdminMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var user = context.User;
            var role = user.FindFirstValue(ClaimTypes.Role);

            if (role == "Admin")
            {
                if (!context.Request.Path.StartsWithSegments("/Auth/Logout") &&
                    !context.Request.Path.StartsWithSegments("/Admin") &&
                    !context.Request.Path.StartsWithSegments("/Product") &&
                    !context.Request.Path.StartsWithSegments("/Order") &&
                    !context.Request.Path.StartsWithSegments("/Category") &&
                    !context.Request.Path.StartsWithSegments("/Users") &&
                    !context.Request.Path.StartsWithSegments("/Feedback") &&
                    !context.Request.Path.StartsWithSegments("/Review") &&
                    !context.Request.Path.StartsWithSegments("/Coupon") 

                    )
                {
                    context.Response.Redirect("/Admin/Index");
                    return;
                }
            }

            await _next(context);
        }

    }

}
