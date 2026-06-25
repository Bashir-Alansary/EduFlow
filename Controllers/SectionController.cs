using EduFlow.Entities;
using EduFlow.Entities.Constants;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.ViewModels.Sections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduFlow.Controllers
{
    public class SectionController : Controller
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public SectionController(
            ISectionRepository sectionRepository,
            ICourseRepository courseRepository,
            UserManager<ApplicationUser> userManager)
        {
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> Create(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);

            if (course == null)
                return NotFound();

            var vm = new CreateSectionVM
            {
                CourseId = courseId
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSectionVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var section = new Section
            {
                Title = vm.Title,
                Order = vm.Order,
                CourseId = vm.CourseId
            };

            await _sectionRepository.AddAsync(section);

            return RedirectToAction(
                "Details",
                "Courses",
                new { id = vm.CourseId });
        }

        [HttpGet]
        [Authorize(Roles = Roles.Instructor)]
        public async Task<IActionResult> Edit(int id)
        {
            var section = await _sectionRepository.GetByIdAsync(id);

            if (section == null)
                return NotFound();

            var vm = new CreateSectionVM
            {
                Id = section.Id,
                Title = section.Title,
                Order = section.Order,
                CourseId = section.CourseId
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Instructor)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var section = await _sectionRepository.GetByIdAsync(id);

            if (section == null)
                return NotFound();

            var courseId = section.CourseId;

            _sectionRepository.Delete(section);

            return RedirectToAction(
                "Details",
                "Courses",
                new { id = courseId });
        }
    }
}
