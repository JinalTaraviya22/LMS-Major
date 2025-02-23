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

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LMS_db;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

        public bool InsertUser(AdminModel am)
        {
            if (am.Name != string.Empty && am.Email != string.Empty && am.Password != string.Empty && am.Role != string.Empty)
            {
                SqlCommand cmd = new SqlCommand("insert into User_tbl values(@nm,@email,@password,@role)", con);
                cmd.Parameters.AddWithValue("@nm", am.Name);
                cmd.Parameters.AddWithValue("@email", am.Email);
                cmd.Parameters.AddWithValue("@password", am.Password);
                cmd.Parameters.AddWithValue("@role", am.Role);

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

        public List<AdminModel> showData()
        {
            List<AdminModel> userlist = new List<AdminModel>();

            SqlCommand cmd = new SqlCommand("select * from User_tbl",con);
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
                    Role = dr.GetString(4)
                };
                userlist.Add(user);
            }
            con.Close();
            return userlist;
        }
    }
}

