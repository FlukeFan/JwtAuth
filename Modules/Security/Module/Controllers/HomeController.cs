using Microsoft.AspNetCore.Mvc;

namespace MfePoc.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
