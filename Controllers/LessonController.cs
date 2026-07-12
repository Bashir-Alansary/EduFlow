using EduFlow.Entities;
using EduFlow.Entities.Constants;
using EduFlow.Models;
using EduFlow.Repositories.Implementations;
using EduFlow.Repositories.Interfaces;
using EduFlow.ViewModels.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduFlow.Controllers
{
    public class LessonController : Controller
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ILessonProgressRepository _lessonProgressRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LessonController(
            ILessonRepository lessonRepository,
            ISectionRepository sectionRepository,
            ILessonProgressRepository lessonProgressRepository,
            IEnrollmentRepository enrollmentRepository,
            UserManager<ApplicationUser> userManager)
        {
            _lessonRepository = lessonRepository;
            _sectionRepository = sectionRepository;
            _lessonProgressRepository = lessonProgressRepository;
            _enrollmentRepository = enrollmentRepository;
            _userManager = userManager;
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpGet]
        public async Task<IActionResult> Create(int sectionId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);

            if (section == null)
                return NotFound();

            var vm = new CreateLessonVM
            {
                SectionId = sectionId
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLessonVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var lesson = new Lesson
            {
                Title = vm.Title,
                Content = vm.Content,
                VideoUrl = vm.VideoUrl,
                Order = vm.Order,
                SectionId = vm.SectionId
            };

            var section = await _sectionRepository.GetByIdAsync(vm.SectionId);

            if (section == null)
                return NotFound();

            await _lessonRepository.AddAsync(lesson);

            return RedirectToAction(
                "Details",
                "Courses",
                new { id = section.CourseId });
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);

            if (lesson == null)
                return NotFound();

            var vm = new CreateLessonVM
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                VideoUrl = lesson.VideoUrl,
                Order = lesson.Order,
                SectionId = lesson.SectionId
            };

            return View(vm);
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateLessonVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var lesson = await _lessonRepository.GetByIdAsync(vm.Id);

            if (lesson == null)
                return NotFound();

            lesson.Title = vm.Title;
            lesson.Content = vm.Content;
            lesson.VideoUrl = vm.VideoUrl;
            lesson.Order = vm.Order;

            _lessonRepository.Update(lesson);

            var section = await _sectionRepository.GetByIdAsync(vm.SectionId);

            if (section == null)
                return NotFound();

            return RedirectToAction(
                "Details",
                "Courses",
                new { id = section.CourseId });
        }

        [Authorize(Roles = Roles.Instructor)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);

            if (lesson == null)
                return NotFound();

            var section = await _sectionRepository.GetByIdAsync(lesson.SectionId);

            if (section == null)
                return NotFound();

            _lessonRepository.Delete(lesson);

            return RedirectToAction(
                "Details",
                "Courses",
                new { id = section.CourseId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);

            if (lesson == null)
                return NotFound();

            if (User.IsInRole(Roles.Student))
            {
                var studentId = _userManager.GetUserId(User);

                var isEnrolled = await _enrollmentRepository.IsEnrolledAsync(
                    studentId,
                    lesson.Section.CourseId);

                if (!isEnrolled)
                    return Forbid();
            }

            bool isCompleted = false;

            if (User.Identity!.IsAuthenticated)
            {
                var studentId = _userManager.GetUserId(User);

                if (!string.IsNullOrEmpty(studentId))
                {
                    var progress = await _lessonProgressRepository
                        .GetAsync(studentId, lesson.Id);

                    isCompleted = progress?.IsCompleted ?? false;
                }
            }

            var vm = new LessonDetailsVM
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                VideoUrl = lesson.VideoUrl,
                Order = lesson.Order,

                SectionId = lesson.SectionId,
                SectionTitle = lesson.Section.Title,

                CourseId = lesson.Section.CourseId,
                CourseTitle = lesson.Section.Course.Title,

                IsCompleted = isCompleted
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Student)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int lessonId)
        {
            var studentId = _userManager.GetUserId(User);

            if (studentId == null)
                return Unauthorized();

            var lesson = await _lessonRepository.GetByIdAsync(lessonId);

            if (lesson == null)
                return NotFound();

            var progress = await _lessonProgressRepository.GetAsync(studentId, lessonId);

            if (progress == null)
            {
                progress = new LessonProgress
                {
                    StudentId = studentId,
                    LessonId = lessonId,
                    ProgressPercent = 100,
                    IsCompleted = true,
                    LastWatchedAt = DateTime.UtcNow
                };

                await _lessonProgressRepository.AddAsync(progress);
            }
            else
            {
                progress.ProgressPercent = 100;
                progress.IsCompleted = true;
                progress.LastWatchedAt = DateTime.UtcNow;

                _lessonProgressRepository.Update(progress);
            }

            var course = lesson.Section.Course;

            var totalLessons = course.Sections
                .SelectMany(s => s.Lessons)
                .Count();

            var completedLessons = course.Sections
                .SelectMany(s => s.Lessons)
                .Count(l => l.LessonProgresses.Any(lp =>
                    lp.StudentId == studentId &&
                    lp.IsCompleted));

            var enrollment = await _enrollmentRepository
                .GetAsync(studentId, course.Id);

            if (enrollment != null)
            {
                enrollment.ProgressPercentage =
                    totalLessons == 0
                        ? 0
                        : (int)Math.Round((double)completedLessons * 100 / totalLessons);

                enrollment.IsCompleted = completedLessons == totalLessons;

                _enrollmentRepository.Update(enrollment);
            }

            return RedirectToAction("Details", new { id = lessonId });
        }

    }
}
