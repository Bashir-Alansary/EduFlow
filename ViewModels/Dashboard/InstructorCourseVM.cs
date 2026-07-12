namespace EduFlow.ViewModels.Dashboard
{
    public class InstructorCourseVM
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int StudentsCount { get; set; }

        public int SectionsCount { get; set; }

        public int LessonsCount { get; set; }

        public double AverageRating { get; set; }
    }
}
