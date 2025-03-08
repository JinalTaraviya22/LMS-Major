using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace Major.Models
{
    public class CourseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string AcadamicYear { get; set; }
        public string Program { get; set; }
        public int Credit { get; set; }
        public int Semester { get; set; }
        public string Image { get; set; }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LMS_db;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

        public bool insertCourse(CourseModel cm, IFormFile ImageFile)
        {
            if (cm.Title != string.Empty && cm.Code != string.Empty && cm.AcadamicYear != string.Empty && cm.Program != string.Empty && cm.Credit != 0 && cm.Semester != 0 && (ImageFile != null && ImageFile.Length > 0))
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Course_img");

                // Ensure the Uploads directory exists
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fileStream);
                }

                string imagePath = "/Image/Course_img/" + uniqueFileName;

                // Save the image path in the database
                string query = "insert into Course_tbl values(@title,@code,@acadamic_yr,@program,@credit,@sem,@image)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@title", cm.Title);
                    cmd.Parameters.AddWithValue("@code", cm.Code);
                    cmd.Parameters.AddWithValue("@acadamic_yr", cm.AcadamicYear);
                    cmd.Parameters.AddWithValue("@program", cm.Program);
                    cmd.Parameters.AddWithValue("@credit", cm.Credit);
                    cmd.Parameters.AddWithValue("@sem", cm.Semester);
                    cmd.Parameters.AddWithValue("@image", imagePath);
                    con.Open();
                    int res = cmd.ExecuteNonQuery();
                    if (res >= 1)
                    {
                        con.Close();
                        return true;
                    }
                    else
                    {
                        con.Close();
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public List<CourseModel> showCourseData(string? acadamicYear = null, string? searchQuery = null, int? limit = null)
        {
            List<CourseModel> courseList = new List<CourseModel>();

            string query = "SELECT * FROM Course_tbl WHERE 1=1"; // Ensures flexible filtering

            if (!string.IsNullOrEmpty(acadamicYear))
            {
                query += " AND C_Acadamic_Year LIKE @acadamicYear";
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query += " AND (C_Title LIKE @search OR C_Code LIKE @search OR C_Program LIKE @search OR C_Semester LIKE @search OR C_Credit LIKE @search)";
            }

            query += " ORDER BY C_Id"; // Ensures proper ordering

            if (limit.HasValue)
            {
                query += " OFFSET 0 ROWS FETCH NEXT @limit ROWS ONLY"; // SQL Server pagination
            }

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (!string.IsNullOrEmpty(acadamicYear))
                {
                    cmd.Parameters.AddWithValue("@acadamicYear", acadamicYear);
                }

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    cmd.Parameters.AddWithValue("@search", $"%{searchQuery}%");
                }

                if (limit.HasValue)
                {
                    cmd.Parameters.AddWithValue("@limit", limit);
                }

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    CourseModel course = new CourseModel()
                    {
                        Id = dr.GetInt32(0),
                        Title = dr.GetString(1),
                        Code = dr.GetString(2),
                        AcadamicYear = dr.GetString(3),
                        Program = dr.GetString(4),
                        Credit = dr.GetInt32(5),
                        Semester = dr.GetInt32(6),
                        Image = dr.GetString(7)
                    };
                    courseList.Add(course);
                }
                con.Close();
            }
            return courseList;
        }
       
        public bool UpdateCourse(CourseModel course, IFormFile? imageFile, string ExistingImage)
        {
            string? newFileName = ExistingImage; // Keep existing image if no new file is uploaded

            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/Course_img");

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a new unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Delete old image if it exists (optional)
                if (!string.IsNullOrEmpty(course.Image))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", course.Image.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                // Save the new image
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                // Store only the relative path
                newFileName = "/Image/Course_img/" + uniqueFileName;
            }

            using (SqlCommand cmd = new SqlCommand(@"
        UPDATE Course_tbl 
        SET C_Title = @title, 
            C_Code = @code, 
            C_Acadamic_Year = @acadamicYear, 
            C_Program = @program, 
            C_Credit = @credit, 
            C_Semester = @semester, 
            C_Image = @image 
        WHERE C_Id = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", course.Id);
                cmd.Parameters.AddWithValue("@title", course.Title);
                cmd.Parameters.AddWithValue("@code", course.Code);
                cmd.Parameters.AddWithValue("@acadamicYear", course.AcadamicYear);
                cmd.Parameters.AddWithValue("@program", course.Program);
                cmd.Parameters.AddWithValue("@credit", course.Credit);
                cmd.Parameters.AddWithValue("@semester", course.Semester);
                cmd.Parameters.AddWithValue("@image", newFileName);
                // Use proper handling for NULL values
                //cmd.Parameters.Add("@image", System.Data.SqlDbType.NVarChar).Value = (object?)newFileName ?? DBNull.Value;

                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();

                return result > 0;
            }
        }
        public bool removeCourse(int courseId)
        {
            string query = "DELETE FROM Course_tbl WHERE C_Id = @CourseId";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                return rowsAffected > 0; // Returns true if a course was deleted, false otherwise
            }
        }

        //count total no of course
        public int courseCount()
        {
            int count=0;
            string query = "select count(*) from Course_tbl";

            using (SqlCommand cmd=new SqlCommand(query, con))
            {
                con.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return count;
        }
    }
}
