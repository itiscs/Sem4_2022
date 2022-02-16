using Microsoft.AspNetCore.Mvc;

namespace MVCFirst.Controllers
{
    public class SecondController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }
    }
}
