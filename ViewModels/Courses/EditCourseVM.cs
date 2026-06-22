using EduFlow.Entities.Enums;
using EduFlow.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduFlow.ViewModels.Courses
{
    public class EditCourseVM : ICourseViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }
        public int DurationInHours { get; set; }

        public CourseLevel Level { get; set; }

        public int CategoryId { get; set; }

        public string? ExistingImage { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(2 * 1024 * 1024)]
        public IFormFile? Image { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
    }

}
