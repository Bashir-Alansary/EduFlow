using EduFlow.ViewModels.Reviews;
using EduFlow.ViewModels.Sections;

namespace EduFlow.ViewModels.Courses
{
    public class CourseDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string Level { get; set; } = null!;
        public string CategoryName { get; set; } = null!;

        public string InstructorName { get; set; } = null!;
        public string? InstructorBio { get; set; }

        public int StudentsCount { get; set; }
        public int SectionsCount { get; set; }
        public int LessonsCount { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsEnrolled { get; set; }
        public bool IsInWishlist { get; set; }
        public List<ReviewVM> Reviews { get; set; } = new ();
        public bool HasReviewed { get; set; }
        public List<SectionVM> Sections { get; set; } = new();
        public int CompletedLessons { get; set; }

        public int TotalLessons { get; set; }

        public int ProgressPercentage { get; set; }
    }
}
