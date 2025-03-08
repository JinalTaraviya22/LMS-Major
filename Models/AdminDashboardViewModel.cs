namespace Major.Models
{
    public class AdminDashboardViewModel
    {
        public List<AdminModel> Students { get; set; } = new List<AdminModel>();
        public List<CourseModel> Courses { get; set; } = new List<CourseModel>();
    }
}
