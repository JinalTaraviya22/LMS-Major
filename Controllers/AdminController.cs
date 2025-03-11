using Major.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Major.Controllers
{
    public class AdminController : Controller
    {
        AdminModel admin = new AdminModel(); //for managing user user
        CourseModel course = new CourseModel();//for managing course
        CourseEnroll ce=new CourseEnroll();//for enrollment
        public IActionResult AdIndex()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userEmail = HttpContext.Session.GetString("UserEmail");
            string? userName = HttpContext.Session.GetString("UserName");
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == "Admin")
            {
                var viewModel = new AdminDashboardViewModel
                {
                    Students = admin.showData(role: "Student", limit: 4),
                    Courses = course.showCourseData(acadamicYear: null, limit: 4)
                };
                TempData["CourseCount"] = course.courseCount();
                TempData["StudentCount"] = admin.countUsers("Student");
                TempData["TeacherCount"] = admin.countUsers("Teacher");
                return View(viewModel);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCourse(CourseModel crm, IFormFile ImageFile)
        {
            if (ImageFile == null || ImageFile.Length == 0)
            {
                TempData["msg"] = "Please attach file";
                return RedirectToAction("ManageCourse");
            }
            bool res = course.insertCourse(crm, ImageFile);
            TempData["msg"] = res ? "Course Created Successfully!" : "Error in creating course.";
            return RedirectToAction("ManageCourse");
        }
        public IActionResult ManageCourse(string searchQuery, int page = 1, int pageSize = 3)
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            List<CourseModel> allCourses = course.showCourseData(searchQuery: searchQuery);

            int totalUsers = allCourses.Count;
            List<CourseModel> courses = allCourses.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            ViewBag.SearchQuery = searchQuery;
            return View(courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCourse(CourseModel course, IFormFile? imageFile, string ExistingImage)
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            bool res = course.UpdateCourse(course, imageFile, ExistingImage);
            TempData["msg"] = res ? "Course Updated Successfully!" : "Error in updating course.";

            return RedirectToAction("ManageCourse");
        }

        public IActionResult DeleteCourse(int id)
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }

            bool res = course.removeCourse(id);
            TempData["msg"] = res ? "Course Deleted Successfully!" : "Error in Deleting Course.";

            return RedirectToAction("ManageCourse");
        }

        public IActionResult AdModules(int courseId)
        {
            string? userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Auth");
            }
            TempData["CourseId"] = courseId;
            List<CourseEnroll> courseEnrollList = ce.showEnrolledStud(cid:courseId);
            return View(courseEnrollList);
        }

        //enroll users in course
        [HttpPost]
        public IActionResult BulkEnroll(int courseId, string userEmails)
        {
            if (string.IsNullOrWhiteSpace(userEmails))
            {
                TempData["msg"] = "Please enter at least one student email.";
                return RedirectToAction("ManageCourse", "Admin");
            }
            var emailList = userEmails.Split(',')
                                         .Select(e => e.Trim().ToLower())
                                         .Distinct()
                                         .ToList();
            int enrolledCount = 0;
            foreach (var email in emailList)
            {
                var newEnrollment = new CourseEnroll
                {
                    CourseId = courseId,
                    UserEmail = email,
                    Status = "Pending"
                };

                bool isSuccess = ce.insert(newEnrollment);
                if (isSuccess)
                    enrolledCount++;
            }

            TempData["msg"] = $"{enrolledCount} students enrolled successfully!";
            return RedirectToAction("ManageCourse", "Admin");
        }

    }
}