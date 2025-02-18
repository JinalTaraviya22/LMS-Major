using Microsoft.AspNetCore.Mvc;

namespace Major.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdIndex()
        {
            return View();
        }
    }
}
