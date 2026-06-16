using EduFlow.Entities;
using EduFlow.Repositories.Implementations;
using EduFlow.Repositories.Interfaces;
using EduFlow.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduFlow.Controllers
{
    public class CoursesController : Controller
    {
        private ICourseRepository _courseRepository;
        private ICategoryRepository _categoryRepository;
        public CoursesController(
            ICourseRepository courseRepository,
            ICategoryRepository categoryRepository
            )
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllAsync();
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();

            var vm = new CreateCourseVM
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();

                vm.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });

                return View(vm);
            }
            var course = new Course
            {
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                Level = vm.Level,
                CategoryId = vm.CategoryId
            };

            await _courseRepository.AddAsync(course);

            return RedirectToAction(nameof(Index));
        }
   
    }
}
