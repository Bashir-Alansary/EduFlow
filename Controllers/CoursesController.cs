using EduFlow.Entities;
using EduFlow.Entities.Constants;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.ViewModels.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduFlow.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CoursesController(
            ICourseRepository courseRepository,
            ICategoryRepository categoryRepository,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Courses (Public)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllAsync();
            return View(courses);
        }

        // GET: Create
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateCourseVM();
            await SetCategories(vm);
            return View(vm);
        }

        // POST: Create
        [Authorize(Roles = Roles.Instructor)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                await SetCategories(vm);
                return View(vm);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var course = new Course
            {
                InstructorId = currentUser!.Id,
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                Level = vm.Level,
                CategoryId = vm.CategoryId
            };

            if (vm.ImageFile != null)
            {
                course.ImageUrl = await SaveImageAsync(vm.ImageFile);
            }

            await _courseRepository.AddAsync(course);

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);

            if (course == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || course.InstructorId != currentUser.Id)
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

        // POST: Edit
        [Authorize(Roles = Roles.Instructor)]
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

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || course.InstructorId != currentUser.Id)
                return Forbid();

            course.Title = vm.Title;
            course.Description = vm.Description;
            course.Price = vm.Price;
            course.DurationInHours = vm.DurationInHours;
            course.CategoryId = vm.CategoryId;
            course.Level = vm.Level;

            if (vm.Image != null)
            {
                DeleteImage(course.ImageUrl);
                course.ImageUrl = await SaveImageAsync(vm.Image);
            }

            _courseRepository.Update(course);

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);

            if (course == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || course.InstructorId != currentUser.Id)
                return Forbid();

            return View(course);
        }

        // POST: Delete
        [Authorize(Roles = Roles.Instructor)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);

            if (course == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || course.InstructorId != currentUser.Id)
                return Forbid();

            DeleteImage(course.ImageUrl);

            _courseRepository.Delete(course);

            return RedirectToAction(nameof(Index));
        }

        // GET: My Courses
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> MyCourses()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            var courses = await _courseRepository
                .GetByInstructorIdAsync(currentUser.Id);

            return View(courses);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseRepository.GetDetailsAsync(id);

            if (course == null)
                return NotFound();

            var vm = new CourseDetailsVM
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                ImageUrl = course.ImageUrl,

                CategoryName = course.Category?.Name,
                InstructorName = course.Instructor?.FullName ?? "Unknown",

                SectionsCount = course.Sections?.Count ?? 0,
                LessonsCount = course.Sections?.Sum(s => s.Lessons?.Count()) ?? 0,
                ReviewsCount = course.Reviews?.Count ?? 0
            };

            return View(vm);
        }

        // ================= Helpers =================

        private async Task SetCategories(ICourseViewModel vm)
        {
            var categories = await _categoryRepository.GetAllAsync();

            vm.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/courses");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return "/images/courses/" + fileName;
        }

        private void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            var path = Path.Combine(
                _webHostEnvironment.WebRootPath,
                imageUrl.TrimStart('/')
            );

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}