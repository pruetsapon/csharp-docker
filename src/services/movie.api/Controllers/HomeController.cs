using Microsoft.AspNetCore.Mvc;

namespace movie.api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
