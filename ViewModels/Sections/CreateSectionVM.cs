using System.ComponentModel.DataAnnotations;

namespace EduFlow.ViewModels.Sections
{
    public class CreateSectionVM
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Order { get; set; }

        public int CourseId { get; set; }
    }
}
