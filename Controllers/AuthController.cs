using Major.Models;
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
                if (user.Role == "Student")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (user.Role == "Teacher")
                {
                    return RedirectToAction("Index", "Teacher");
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
    }
}
