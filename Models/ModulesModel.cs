using Microsoft.Data.SqlClient;

namespace Major.Models
{
    public class ModulesModel
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public int CourseId{ get; set; }

        SqlConnection con = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LMS_db;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

        public bool InsertModule(ModulesModel md)
        {
            string query = "insert into Module_tbl values(@moduleNm,@courseId)";
            using (SqlCommand cmd=new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("moduleNm", md.ModuleName);
                cmd.Parameters.AddWithValue("courseId",md.CourseId);

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

        public List<ModulesModel> showCourseData(int? cid = null)
        {
            List<ModulesModel> moduleList = new List<ModulesModel>();

            string query = "SELECT * FROM Module_tbl WHERE 1=1 AND Course_Id=@course_id"; // Ensures flexible filtering

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("course_id",cid);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ModulesModel mod = new ModulesModel()
                    {
                        Id = dr.GetInt32(0),
                        ModuleName = dr.GetString(1),
                        CourseId = dr.GetInt32(2),
                    };
                    moduleList.Add(mod);
                }
                con.Close();
            }
            return moduleList;
        }
    }
}
