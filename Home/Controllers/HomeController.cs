using Microsoft.AspNetCore.Mvc;

namespace MfePoc.Home.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
