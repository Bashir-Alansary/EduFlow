using EduFlow.Entities;
using EduFlow.Models;
using EduFlow.Repositories.Implementations;
using EduFlow.Repositories.Interfaces;
using EduFlow.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduFlow.Controllers
{
    public class CoursesController : Controller
    {
        private ICourseRepository _courseRepository;
        private ICategoryRepository _categoryRepository;
        private UserManager<ApplicationUser> _userManager;

        public CoursesController(
                ICourseRepository courseRepository,
                ICategoryRepository categoryRepository,
                UserManager<ApplicationUser> userManager
            )
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllAsync();
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateCourseVM();
            
            await SetCategories(vm);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                await SetCategories(vm);

                return View(vm);
            }

            // TEMP: will be replaced with logged-in instructor after Identity integration
            var instructor = await _userManager.FindByEmailAsync("instructor@eduflow.com");
            var course = new Course
            {
                InstructorId = instructor!.Id,
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                Level = vm.Level,
                CategoryId = vm.CategoryId
            };

            await _courseRepository.AddAsync(course);

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);

            if (course == null)
                return NotFound();

            //var instructor = await _userManager.GetUserAsync(User);

            //if (course.InstructorId != instructor.Id)
            //    return Forbid();

            var vm = new EditCourseVM
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                DurationInHours = course.DurationInHours,
                Level = course.Level,
                CategoryId = course.CategoryId,
                ExistingImage = course.ImageUrl
            };

            await SetCategories(vm);

            return View(vm);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                await SetCategories(vm);
                return View(vm);
            }
            var course = await _courseRepository.GetByIdAsync(vm.Id);

            if (course == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (course.InstructorId != user?.Id)
                return Forbid();

            course.Title = vm.Title;
            course.Description = vm.Description;
            course.Price = vm.Price;
            course.DurationInHours = vm.DurationInHours;
            course.CategoryId = vm.CategoryId;
            course.Level = vm.Level;

            _courseRepository.Update(course);

            return RedirectToAction(nameof(Index));
        }



        // Helper method to set categories in the ViewModel
        private async Task SetCategories(ICourseViewModel vm)
        {
            var categories = await _categoryRepository.GetAllAsync();

            vm.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }

    }
}
