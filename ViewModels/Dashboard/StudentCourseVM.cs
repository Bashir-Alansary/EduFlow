namespace EduFlow.ViewModels.Dashboard
{
    public class StudentCourseVM
    {
        public int CourseId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int ProgressPercentage { get; set; }

        public bool IsCompleted { get; set; }
    }
}
