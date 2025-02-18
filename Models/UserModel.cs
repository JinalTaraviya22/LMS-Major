using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Major.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LMS_db;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

        public bool CheckUser(UserModel um)
        {
            if (um.Email != string.Empty && um.Password != string.Empty)
            {
                SqlCommand cmd = new SqlCommand("select * from User_tbl where Email=@mail AND Password=@pwd", con);
                cmd.Parameters.AddWithValue("@mail", um.Email);
                cmd.Parameters.AddWithValue("@pwd", um.Password);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        um.Id = Convert.ToInt32(dr["Id"]);
                        um.Email = dr["Email"].ToString();
                        um.Role = dr["Role"].ToString();
                    }

                    con.Close();
                    return true; // User found and authenticated
                }
                else
                {
                    con.Close();
                    return false; // No matching user found
                }
            }
            return false;
        }
    }
}

