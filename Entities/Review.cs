using EduFlow.Models;
using System.ComponentModel.DataAnnotations;

namespace EduFlow.Entities
{
    public class Review
    {
        public int Id { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string StudentId { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
