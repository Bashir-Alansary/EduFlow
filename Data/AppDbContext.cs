using EduFlow.Entities;
using EduFlow.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EduFlow.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Set default value for CreatedDate in Course entity
            builder.Entity<Course>()
                .Property(c => c.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            // Configure many-to-many relationship between Student and Course through Enrollment
            builder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            // Configure many-to-many relationship between Student and Course through Wishlist
            builder.Entity<Wishlist>()
                .HasKey(w => new { w.StudentId, w.CourseId });

            // Configure many-to-many relationship between Student and Lesson through LessonProgress
            builder.Entity<LessonProgress>()
                .HasKey(lp => new { lp.StudentId, lp.LessonId });

            // Configure one-to-many relationship between Student and Review
            builder.Entity<Review>()
                .HasIndex(r  => new { r.StudentId, r.CourseId })
                .IsUnique();
        }
    }
}
