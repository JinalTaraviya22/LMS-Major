using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;

namespace Major.Models
{
    public class CourseEnroll
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string UserEmail { get; set; }
        public string Status { get; set; }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LMS_db;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        public bool insert(CourseEnroll ce)
        {
            string query = "INSERT INTO Course_Enroll_tbl (CE_Course_Id, CE_User_Email, CE_Status) VALUES (@cid, @uid, @status)";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@cid", ce.CourseId);
                cmd.Parameters.AddWithValue("@uid", ce.UserEmail); // Fixed parameter name
                cmd.Parameters.AddWithValue("@status", ce.Status ?? "Pending");

                try
                {
                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    con.Close();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }

        public List<CourseEnroll> showEnrolledStud(int? cid)
        {
            List<CourseEnroll> courseEnrollList = new List<CourseEnroll>();

            string query = "SELECT * FROM Course_Enroll_tbl WHERE 1=1";
            if (cid != null)
            {
                query += "AND CE_Course_Id=@cid";
            }
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (cid != null)
                {
                    cmd.Parameters.AddWithValue("@cid", cid);
                }

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    CourseEnroll ce = new CourseEnroll()
                    {
                        Id = dr.GetInt32(0),
                        UserEmail = dr.GetString(2),
                        Status = dr.GetString(3)
                    };
                    courseEnrollList.Add(ce);
                }
                con.Close();
            }

            return courseEnrollList;
        }
    }
}
