using EduFlow.Data;
using EduFlow.Entities;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Repositories.Implementations
{
    public class LessonProgressRepository : ILessonProgressRepository
    {
        private readonly AppDbContext _context;

        public LessonProgressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LessonProgress?> GetAsync(string studentId, int lessonId)
        {
            return await _context.LessonProgresses
                .FirstOrDefaultAsync(lp =>
                    lp.StudentId == studentId &&
                    lp.LessonId == lessonId);
        }

        public async Task AddAsync(LessonProgress progress)
        {
            await _context.LessonProgresses.AddAsync(progress);
            await _context.SaveChangesAsync();
        }

        public void Update(LessonProgress progress)
        {
            _context.LessonProgresses.Update(progress);
            _context.SaveChanges();
        }

        public void Delete(LessonProgress progress)
        {
            _context.LessonProgresses.Remove(progress);
            _context.SaveChanges();
        }

        public async Task<int> GetCompletedLessonsCountAsync(string studentId, int courseId)
        {
            return await _context.LessonProgresses
                .CountAsync(lp =>
                    lp.StudentId == studentId &&
                    lp.IsCompleted &&
                    lp.Lesson.Section.CourseId == courseId);
        }

        public async Task<int> GetTotalLessonsCountAsync(int courseId)
        {
            return await _context.Lessons
                .CountAsync(l => l.Section.CourseId == courseId);
        }
    }
}
