using Microsoft.AspNetCore.Mvc;

namespace Major.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult TeachIndex()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userEmail = HttpContext.Session.GetString("UserEmail");
            string? userName = HttpContext.Session.GetString("UserName");
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Teacher")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }
    }
}
