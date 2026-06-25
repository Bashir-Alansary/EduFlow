using EduFlow.Data;
using EduFlow.Entities;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasReviewedAsync(string studentId, int courseId)
        {
            return await _context.Reviews
                .AnyAsync(r =>
                    r.StudentId == studentId &&
                    r.CourseId == courseId);
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId)
        {
            return await _context.Reviews
                .Where(r => r.CourseId == courseId)
                .Include(r => r.Student)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetStudentReviewsCountAsync(string studentId)
        {
            return await _context.Reviews
                .CountAsync(r => r.StudentId == studentId);
        }
    }
}
