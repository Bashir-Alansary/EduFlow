using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduFlow.ViewModels
{
    public interface ICourseViewModel
    {
        IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
