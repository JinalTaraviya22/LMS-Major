using Microsoft.Data.SqlClient;
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
            string query = "INSERT INTO Course_Enroll_tbl (CourseId, UserEmail, Status) VALUES (@cid, @uid, @status)";

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

    }
}
