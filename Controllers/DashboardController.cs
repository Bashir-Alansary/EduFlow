using EduFlow.Entities.Constants;
using EduFlow.Models;
using EduFlow.Repositories.Implementations;
using EduFlow.Repositories.Interfaces;
using EduFlow.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduFlow.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICourseRepository _courseRepository;

        public DashboardController(
            IEnrollmentRepository enrollmentRepository,
            IWishlistRepository wishlistRepository,
            IReviewRepository reviewRepository,
            UserManager<ApplicationUser> userManager,
            ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _wishlistRepository = wishlistRepository;
            _reviewRepository = reviewRepository;
            _userManager = userManager;
            _courseRepository = courseRepository;
        }
        [Authorize(Roles = Roles.Student)]
        public async Task<IActionResult> Student()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            var enrollments =
                await _enrollmentRepository.GetByStudentIdAsync(userId);

            var wishlistCount =
                await _wishlistRepository.GetCountAsync(userId);

            var reviewsCount =
                await _reviewRepository.GetStudentReviewsCountAsync(userId);

            var vm = new StudentDashboardVM
            {
                EnrolledCoursesCount = enrollments.Count(),

                CompletedCoursesCount = enrollments.Count(e => e.IsCompleted),

                WishlistCount = wishlistCount,

                ReviewsCount = reviewsCount,

                Courses = enrollments
                    .Select(e => new StudentCourseVM
                    {
                        CourseId = e.CourseId,
                        Title = e.Course.Title,
                        ProgressPercentage = e.ProgressPercentage,
                        IsCompleted = e.IsCompleted
                    })
                    .ToList()
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> Instructor()
        {
            var instructorId = _userManager.GetUserId(User);

            if (instructorId == null)
                return Unauthorized();

            var courses = await _courseRepository.GetByInstructorIdAsync(instructorId);

            var vm = new InstructorDashboardVM
            {
                CoursesCount = courses.Count,

                StudentsCount = courses.Sum(c => c.Enrollments.Count),

                SectionsCount = courses.Sum(c => c.Sections.Count),

                LessonsCount = courses.Sum(c => c.Sections.Sum(s => s.Lessons.Count)),

                ReviewsCount = courses.Sum(c => c.Reviews.Count),

                Courses = courses.Select(c => new InstructorCourseVM
                {
                    Id = c.Id,

                    Title = c.Title,

                    StudentsCount = c.Enrollments.Count,

                    SectionsCount = c.Sections.Count,

                    LessonsCount = c.Sections.Sum(s => s.Lessons.Count),

                    AverageRating = c.Reviews.Any()
                        ? c.Reviews.Average(r => r.Rating)
                        : 0
                }).ToList()
            };

            return View(vm);
        }
    }
}
