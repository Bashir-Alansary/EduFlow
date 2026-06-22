using EduFlow.Entities.Enums;
using EduFlow.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EduFlow.ViewModels.Courses
{
    public class CreateCourseVM : ICourseViewModel
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public string Description { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal Price { get; set; }

        [Required]
        public CourseLevel Level { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(2 * 1024 * 1024)]
        public IFormFile? ImageFile { get; set; }

        // For dropdown only (GET)
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }

}
