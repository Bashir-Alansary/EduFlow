namespace EduFlow.ViewModels.Dashboard
{
    public class InstructorDashboardVM
    {
        public int CoursesCount { get; set; }

        public int StudentsCount { get; set; }

        public int SectionsCount { get; set; }

        public int LessonsCount { get; set; }

        public int ReviewsCount { get; set; }

        public List<InstructorCourseVM> Courses { get; set; } = new();
    }
}
