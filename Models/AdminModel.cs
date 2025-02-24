using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;

namespace Major.Models
{
    public class AdminModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public string Status { get; set; }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LMS_db;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

        public bool InsertUser(AdminModel am)
        {
            if (am.Name != string.Empty && am.Email != string.Empty && am.Password != string.Empty && am.Role != string.Empty)
            {
                SqlCommand cmd = new SqlCommand("insert into User_tbl values(@nm,@email,@password,@role,@status)", con);
                cmd.Parameters.AddWithValue("@nm", am.Name);
                cmd.Parameters.AddWithValue("@email", am.Email);
                cmd.Parameters.AddWithValue("@password", am.Password);
                cmd.Parameters.AddWithValue("@role", am.Role);
                cmd.Parameters.AddWithValue("@status",am.Status);
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
            else
            {
                return false;
            }
        }

        //public List<AdminModel> showData()
        //{
        //    List<AdminModel> userlist = new List<AdminModel>();

        //    SqlCommand cmd = new SqlCommand("select * from User_tbl",con);
        //    con.Open();
        //    SqlDataReader dr = cmd.ExecuteReader();

        //    while (dr.Read())
        //    {
        //        AdminModel user = new AdminModel()
        //        {
        //            Id = dr.GetInt32(0),
        //            Name = dr.GetString(1),
        //            Email = dr.GetString(2),
        //            Password = dr.GetString(3),
        //            Role = dr.GetString(4)
        //        };
        //        userlist.Add(user);
        //    }
        //    con.Close();
        //    return userlist;
        //}

        public List<AdminModel> showData(string? role = null, string? searchQuery = null, int? limit = null)
        {
            List<AdminModel> userlist = new List<AdminModel>();

            string query = "SELECT * FROM User_tbl WHERE 1=1"; // Ensures flexible filtering

            if (!string.IsNullOrEmpty(role))
            {
                query += " AND Role = @role";
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query += " AND (Name LIKE @search OR Email LIKE @search OR Role LIKE @search OR Status LIKE @search)";
            }

            if (limit.HasValue)
            {
                query += " ORDER BY Id OFFSET 0 ROWS FETCH NEXT @limit ROWS ONLY"; // SQL Server pagination
            }

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (!string.IsNullOrEmpty(role))
                {
                    cmd.Parameters.AddWithValue("@role", role);
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
                    AdminModel user = new AdminModel()
                    {
                        Id = dr.GetInt32(0),
                        Name = dr.GetString(1),
                        Email = dr.GetString(2),
                        Password = dr.GetString(3),
                        Role = dr.GetString(4),
                        Status=dr.GetString(5)
                    };
                    userlist.Add(user);
                }
                con.Close();
            }

            return userlist;
        }

        public bool UpdateUser(AdminModel user)
        {
            using (SqlCommand cmd = new SqlCommand("UPDATE User_tbl SET Name = @name, Email = @email, Role = @role, Status=@status WHERE Id = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", user.Id);
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@role", user.Role);
                cmd.Parameters.AddWithValue("@status", user.Status);

                con.Open();
                int res = cmd.ExecuteNonQuery();
                con.Close();

                return res > 0;
            }
        }
    }
}

