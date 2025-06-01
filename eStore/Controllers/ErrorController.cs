using Microsoft.AspNetCore.Mvc;

namespace eStore.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
