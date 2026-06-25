using EduFlow.Entities.Constants;
using EduFlow.Models;
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

        public DashboardController(
            IEnrollmentRepository enrollmentRepository,
            IWishlistRepository wishlistRepository,
            IReviewRepository reviewRepository,
            UserManager<ApplicationUser> userManager)
        {
            _enrollmentRepository = enrollmentRepository;
            _wishlistRepository = wishlistRepository;
            _reviewRepository = reviewRepository;
            _userManager = userManager;
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
    }
}
