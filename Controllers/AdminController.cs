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
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Admin")
            {
                List<AdminModel> user = admin.showData(role: "Student", limit: 4);
                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "Auth");
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

        //public IActionResult ManageUsers()
        //{
        //    string? userRole = HttpContext.Session.GetString("UserRole");
        //    if (userRole != "Admin")
        //    {
        //        return RedirectToAction("Login", "Auth");
        //    }
        //    List<AdminModel> user = admin.showData();
        //    return View(user);
        //}
        public IActionResult ManageUsers(string searchQuery, int page = 1, int pageSize = 3)
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            List<AdminModel> allUsers = admin.showData(searchQuery: searchQuery);

            int totalUsers = allUsers.Count;
            List<AdminModel> users = allUsers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            ViewBag.SearchQuery = searchQuery;

            return View(users);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(AdminModel user)
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            Console.WriteLine($"Received: Id={user.Id}, Name={user.Name}, Email={user.Email}");

                bool result = admin.UpdateUser(user);
                if (result)
                {
                    TempData["msg"] = "User updated successfully!";
                }
                else
                {
                    TempData["msg"] = "Error updating user!";
                }

            return RedirectToAction("ManageUsers");
        }

    }
}
