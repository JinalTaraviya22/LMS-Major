using Major.Models;
using Microsoft.AspNetCore.Mvc;

namespace Major.Controllers
{
    public class AdminController : Controller
    {
        AdminModel admin = new AdminModel();
        public IActionResult AdIndex()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userEmail = HttpContext.Session.GetString("UserEmail");
            string? userName = HttpContext.Session.GetString("UserName");
            string? userRole= HttpContext.Session.GetString("UserRole");
            if (userRole == "Admin")
            {
                List<AdminModel> user = admin.showData();
                return View(user);
            }
            else
            {
                return RedirectToAction("Login","Auth");
            }
        }
        public IActionResult AddUsers()
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        public IActionResult ManageUsers()
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }
            List<AdminModel> user = admin.showData();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUsers(AdminModel adm)
        {
            bool res;
            if (ModelState.IsValid)
            {
                res = admin.InsertUser(adm);
                if (res)
                    TempData["msg"] = "Inserted";
                else
                    TempData["msg"] = "Error";
            }
            return View();
        }
    }
}
