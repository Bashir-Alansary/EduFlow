using EduFlow.Data;
using EduFlow.Entities;
using EduFlow.Entities.Constants;
using EduFlow.Entities.Enums;
using EduFlow.Models;
using Microsoft.AspNetCore.Identity;

public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbSeeder(
        AppDbContext context, 
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        // Seed Roles
        if (!await _roleManager.RoleExistsAsync(Roles.Instructor))
        {
            await _roleManager.CreateAsync(new IdentityRole(Roles.Instructor));
        }

        if (!await _roleManager.RoleExistsAsync(Roles.Student))
        {
            await _roleManager.CreateAsync(new IdentityRole(Roles.Student));
        }

        // Seed Categories
        if (!_context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Programming", Description = "Programming courses" },
                new Category { Name = "Design", Description = "Design courses" },
                new Category { Name = "Marketing", Description = "Marketing courses" }
            };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }

        // Seed Instructor User
        var instructor = await _userManager.FindByEmailAsync("instructor@eduflow.com");
        if (instructor == null)
        {
            instructor = new ApplicationUser
            {
                UserName = "instructor@eduflow.com",
                Email = "instructor@eduflow.com",
                FullName = "Test Instructor",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(instructor, "Instructor@123");
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (!await _userManager.IsInRoleAsync(instructor, Roles.Instructor))
            {
                await _userManager.AddToRoleAsync(instructor, Roles.Instructor);
            }
        }

        // Seed Courses
        if (!_context.Courses.Any())
        {
            var programmingCategory = _context.Categories
                .First(c => c.Name == "Programming");

            var courses = new List<Course>
            {
                new Course
                {
                    Title = "C# Basics",
                    Description = "Learn C# from scratch",
                    Price = 100,
                    CategoryId = programmingCategory.Id,
                    InstructorId = instructor.Id,
                    CreatedDate = DateTime.Now,
                    IsPublished = true,
                    DurationInHours = 10,
                    Level = CourseLevel.Beginner
                }
            };

            _context.Courses.AddRange(courses);
            await _context.SaveChangesAsync();
        }
    }
}