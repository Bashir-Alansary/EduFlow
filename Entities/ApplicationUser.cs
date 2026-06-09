using EduFlow.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EduFlow.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;
        public string? ProfileImage { get; set; }
        [StringLength(500)]
        public string? Bio { get; set; }
        public List<Course> CoursesCreated { get; set; } = new List<Course>();
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public List<LessonProgress> Lessons { get; set; } = new List<LessonProgress>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    }
}
