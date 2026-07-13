using EduFlow.Entities;
using EduFlow.Entities.Constants;
using EduFlow.Models;
using EduFlow.Repositories.Implementations;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;
using EduFlow.ViewModels.Courses;
using EduFlow.ViewModels.Lessons;
using EduFlow.ViewModels.Reviews;
using EduFlow.ViewModels.Sections;
using EduFlow.ViewModels.Wishlist;
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
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ILessonProgressRepository _lessonProgressRepository;
        private readonly ICertificateService _certificateService;

        public CoursesController(
            ICourseRepository courseRepository,
            ICategoryRepository categoryRepository,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            IEnrollmentRepository enrollmentRepository,
            IWishlistRepository wishlistRepository,
            IReviewRepository reviewRepository,
            ILessonProgressRepository lessonProgressRepository,
            ICertificateService certificateService)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _enrollmentRepository = enrollmentRepository;
            _wishlistRepository = wishlistRepository;
            _reviewRepository = reviewRepository;
            _lessonProgressRepository = lessonProgressRepository;
            _certificateService = certificateService;
        }

        // GET: Courses (Public)
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId)
        {
            var courses = await _courseRepository.FilterAsync(searchTerm, categoryId);
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);

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

            var userId = User.Identity?.IsAuthenticated == true
                ? _userManager.GetUserId(User)
                : null;

            var isEnrolled = false;
            var isInWishlist = false;
            var hasReviewed = false;

            bool canDownloadCertificate = false;

            if (userId != null)
            {
                isEnrolled = await _enrollmentRepository.IsEnrolledAsync(userId, id);
                isInWishlist = await _wishlistRepository.IsInWishlistAsync(userId, id);
                hasReviewed = await _reviewRepository.HasReviewedAsync(userId, id);

                var enrollment = course.Enrollments
                    .FirstOrDefault(e => e.StudentId == userId);

                if (enrollment != null)
                {
                    canDownloadCertificate = enrollment.IsCompleted;
                }
            }

            var vm = new CourseDetailsVM
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                CanDownloadCertificate = canDownloadCertificate,
                ImageUrl = course.ImageUrl,

                IsEnrolled = isEnrolled,
                IsInWishlist = isInWishlist,
                HasReviewed = hasReviewed,

                CategoryName = course.Category?.Name ?? "Unknown",
                InstructorName = course.Instructor?.FullName ?? "Unknown",
                InstructorBio = course.Instructor?.Bio,

                SectionsCount = course.Sections?.Count ?? 0,
                LessonsCount = course.Sections?.Sum(s => s.Lessons.Count) ?? 0,
                ReviewsCount = course.Reviews?.Count ?? 0,

                Reviews = course.Reviews
                    .Select(r => new ReviewVM
                    {
                        StudentName = r.Student.FullName,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList(),

                Sections = course.Sections
                    .OrderBy(s => s.Order)
                    .Select(s => new SectionVM
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Order = s.Order,

                        LessonsCount = s.Lessons.Count,

                        Lessons = s.Lessons
                            .OrderBy(l => l.Order)
                            .Select(l => new LessonVM
                            {
                                Id = l.Id,
                                Title = l.Title,
                                Order = l.Order
                            })
                            .ToList()
                    })
                    .ToList()
            };

            // Student Progress
            if (userId != null && User.IsInRole(Roles.Student))
            {
                vm.CompletedLessons =
                    await _lessonProgressRepository
                        .GetCompletedLessonsCountAsync(userId, course.Id);

                vm.TotalLessons = vm.LessonsCount;

                if (vm.TotalLessons > 0)
                {
                    vm.ProgressPercentage =
                        (int)Math.Round(
                            (double)vm.CompletedLessons * 100 / vm.TotalLessons);
                }
            }

            return View(vm);
        }

        // POST: Enroll
        [HttpPost]
        [Authorize(Roles = Roles.Student)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = _userManager.GetUserId(User);

            if (await _enrollmentRepository.IsEnrolledAsync(userId, courseId))
                return RedirectToAction("Details", new { id = courseId });

            var enrollment = new Enrollment
            {
                StudentId = userId!,
                CourseId = courseId,
                EnrolledAt = DateTime.Now,
                ProgressPercentage = 0,
                IsCompleted = false
            };

            await _enrollmentRepository.EnrollAsync(enrollment);

            return RedirectToAction("Details", new { id = courseId });
        }

        // POST: Add to Wishlist
        [HttpPost]
        [Authorize(Roles = Roles.Student)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int courseId)
        {
            var userId = _userManager.GetUserId(User);

            if (!await _wishlistRepository.IsInWishlistAsync(userId!, courseId))
            {
                await _wishlistRepository.AddAsync(new Wishlist
                {
                    StudentId = userId!,
                    CourseId = courseId
                });
            }

            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        // POST: Remove from Wishlist
        [HttpPost]
        [Authorize(Roles = Roles.Student)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist(int courseId)
        {
            var userId = _userManager.GetUserId(User);

            await _wishlistRepository.RemoveAsync(userId!, courseId);

            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        // GET: Wishlist
        [HttpGet]
        [Authorize(Roles = Roles.Student)]
        public async Task<IActionResult> Wishlist()
        {
            var userId = _userManager.GetUserId(User);

            var wishlist = await _wishlistRepository
                .GetByStudentIdAsync(userId!);

            var vm = wishlist.Select(w => new WishlistItemVM
            {
                CourseId = w.CourseId,
                Title = w.Course.Title,
                ImageUrl = w.Course.ImageUrl,
                Price = w.Course.Price,
                CategoryName = w.Course.Category?.Name ?? "Unknown"
            });

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Student)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearWishlist()
        {
            var userId = _userManager.GetUserId(User);

            await _wishlistRepository.RemoveAllAsync(userId!);

            return RedirectToAction(nameof(Wishlist));
        }

        // POST: Add Review
        [HttpPost]
        [Authorize(Roles = Roles.Student)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview( int courseId, int rating, string? comment)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            var isEnrolled = await _enrollmentRepository
                .IsEnrolledAsync(userId, courseId);

            if (!isEnrolled)
                return Forbid();

            var hasReviewed = await _reviewRepository
                .HasReviewedAsync(userId, courseId);

            if (hasReviewed)
                return RedirectToAction(nameof(Details),
                    new { id = courseId });

            var review = new Review
            {
                StudentId = userId,
                CourseId = courseId,
                Rating = rating,
                Comment = comment
            };

            await _reviewRepository.AddAsync(review);

            return RedirectToAction(nameof(Details),
                new { id = courseId });
        }

        [HttpGet]
        [Authorize(Roles = Roles.Student)]
        public async Task<IActionResult> DownloadCertificate(int id)
        {
            var studentId = _userManager.GetUserId(User);

            if (studentId == null)
                return Unauthorized();

            var course = await _courseRepository.GetDetailsAsync(id);

            if (course == null)
                return NotFound();

            var enrollment = course.Enrollments
                .FirstOrDefault(e => e.StudentId == studentId);

            if (enrollment == null)
                return Forbid();

            if (!enrollment.IsCompleted)
                return BadRequest("Course is not completed yet.");

            var studentName = User.Identity!.Name!;

            var instructorName = course.Instructor.FullName;

            var pdfBytes = _certificateService.GenerateCertificate(
                studentName,
                course.Title,
                instructorName,
                enrollment.CompletedAt ?? DateTime.Now);

            return File(
                pdfBytes,
                "application/pdf",
                $"{course.Title}-Certificate.pdf");
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