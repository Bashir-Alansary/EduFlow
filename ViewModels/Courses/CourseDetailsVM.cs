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
        public int SectionsCount { get; internal set; }
        public int? LessonsCount { get; internal set; }
        public int ReviewsCount { get; internal set; }
    }
}
