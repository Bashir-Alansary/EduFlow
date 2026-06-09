using EduFlow.Models;

namespace EduFlow.Entities
{
    public class LessonProgress
    {
        public int StudentId { get; set; }
        public ApplicationUser Student { get; set; } = null!;

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;

        public int ProgressPercent { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? LastWatchedAt { get; set; }
    }
}
