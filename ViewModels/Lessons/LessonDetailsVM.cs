namespace EduFlow.ViewModels.Lessons
{
    public class LessonDetailsVM
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }

        public string? VideoUrl { get; set; }

        public int Order { get; set; }

        public int SectionId { get; set; }

        public string SectionTitle { get; set; } = string.Empty;

        public int CourseId { get; set; }

        public string CourseTitle { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public int ProgressPercentage { get; set; }

        public int CompletedLessons { get; set; }

        public int TotalLessons { get; set; }
    }
}
