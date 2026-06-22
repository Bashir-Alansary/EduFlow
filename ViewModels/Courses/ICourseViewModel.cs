using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduFlow.ViewModels.Courses
{
    public interface ICourseViewModel
    {
        IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
