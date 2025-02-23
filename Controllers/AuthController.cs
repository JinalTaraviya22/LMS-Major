using Major.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Major.Controllers
{
    public class AuthController : Controller
    {
        UserModel umodel = new UserModel();
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            bool authCheck = umodel.CheckUser(user);
            if (authCheck)
            {
                //session set
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName",user.Name);
                HttpContext.Session.SetString("UserRole",user.Role);

                if (user.Role == "Student")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (user.Role == "Teacher")
                {
                    return RedirectToAction("TeachIndex", "Teacher");
                }
                else if (user.Role == "Admin")
                {
                    return RedirectToAction("AdIndex", "Admin");
                }
                else
                {
                    return RedirectToAction("Auth", "Login");
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "Invalid email or password.";
                return View(user);
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session
            HttpContext.SignOutAsync();  // Sign out if using authentication

            return RedirectToAction("Login", "Auth");
        }
    }
}
