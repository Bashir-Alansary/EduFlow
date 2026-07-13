using EduFlow.Models;
using System.ComponentModel.DataAnnotations;

namespace EduFlow.Entities
{
    public class Enrollment
    {
        public string StudentId { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public DateTime EnrolledAt { get; set; }
        [Range(0, 100)]
        public int ProgressPercentage { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
