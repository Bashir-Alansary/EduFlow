using EduFlow.Entities;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduFlow.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private ICourseRepository _courseRepository;
        private ICategoryRepository _categoryRepository;
        private UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CoursesController(
                ICourseRepository courseRepository,
                ICategoryRepository categoryRepository,
                UserManager<ApplicationUser> userManager,
                IWebHostEnvironment webHostEnvironment
            )
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Courses
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllAsync();
            return View(courses);
        }

        // GET: Courses/Details/5
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateCourseVM();
            
            await SetCategories(vm);

            return View(vm);
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                await SetCategories(vm);
                return View(vm);
            }

            var instructor = await _userManager.GetUserAsync(User);

            if (instructor == null)
                return Unauthorized();

            var course = new Course
            {
                InstructorId = instructor.Id,
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                Level = vm.Level,
                CategoryId = vm.CategoryId
            };

            // IMAGE UPLOAD
            if (vm.ImageFile != null)
            {
                course.ImageUrl = await SaveImageAsync(vm.ImageFile);
            }

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

            var instructor = await _userManager.GetUserAsync(User);

            if (course.InstructorId != instructor.Id)
                return Forbid();

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

        // GET: Courses/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);

            if (course == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (course.InstructorId != user?.Id)
                return Forbid();

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);

            if (course == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            if (course.InstructorId != user?.Id)
                return Forbid();

            _courseRepository.Delete(course);

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

        // Helper method to save uploaded image and return its URL
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/courses");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return "/images/courses/" + uniqueFileName;
        }

    }
}
