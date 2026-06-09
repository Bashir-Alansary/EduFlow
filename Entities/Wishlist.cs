using EduFlow.Models;

namespace EduFlow.Entities
{
    public class Wishlist
    {
        public string StudentId { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
