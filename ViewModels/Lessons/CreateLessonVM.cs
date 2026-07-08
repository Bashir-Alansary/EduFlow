using System.ComponentModel.DataAnnotations;

namespace EduFlow.ViewModels.Lessons
{
    public class CreateLessonVM
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Content { get; set; }

        public string? VideoUrl { get; set; }

        [Range(1, int.MaxValue)]
        public int Order { get; set; }

        public int SectionId { get; set; }
    }
}
