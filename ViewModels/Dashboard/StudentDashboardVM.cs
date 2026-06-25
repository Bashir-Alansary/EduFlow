namespace EduFlow.ViewModels.Dashboard
{
    public class StudentDashboardVM
    {
        public int EnrolledCoursesCount { get; set; }

        public int CompletedCoursesCount { get; set; }

        public int WishlistCount { get; set; }

        public int ReviewsCount { get; set; }

        public List<StudentCourseVM> Courses { get; set; } = new();
    }
}
