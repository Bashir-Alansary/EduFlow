using EduFlow.Entities;
using EduFlow.Entities.Constants;
using EduFlow.Repositories.Interfaces;
using EduFlow.ViewModels.Lessons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduFlow.Controllers
{
    [Authorize(Roles = Roles.Instructor)]
    public class LessonController : Controller
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ISectionRepository _sectionRepository;

        public LessonController(
            ILessonRepository lessonRepository,
            ISectionRepository sectionRepository)
        {
            _lessonRepository = lessonRepository;
            _sectionRepository = sectionRepository;
        }

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

    }
}
