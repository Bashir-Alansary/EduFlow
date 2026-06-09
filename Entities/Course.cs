using EduFlow.Entities.Enums;
using EduFlow.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduFlow.Entities
{
    public class Course
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = null!;
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 9999999.99)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }
        [Range(1, 1000)]
        public int DurationInHours { get; set; }

        public bool IsPublished { get; set; }

        public CourseLevel Level { get; set; }

        // Foreign Keys
        public string InstructorId { get; set; } = null!;
        public ApplicationUser Instructor { get; set; } = null!;

        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public List<Section> Sections { get; set; } = new List<Section>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
