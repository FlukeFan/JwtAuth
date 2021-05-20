using Microsoft.AspNetCore.Mvc;

namespace AuthEx.Security.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
