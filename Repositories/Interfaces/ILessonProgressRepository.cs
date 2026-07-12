using EduFlow.Entities;

namespace EduFlow.Repositories.Interfaces
{
    public interface ILessonProgressRepository
    {
        Task<LessonProgress?> GetAsync(string studentId, int lessonId);

        Task AddAsync(LessonProgress progress);

        void Update(LessonProgress progress);

        void Delete(LessonProgress progress);

        Task<int> GetCompletedLessonsCountAsync(string studentId, int courseId);
        Task<int> GetTotalLessonsCountAsync(int courseId);
    }
}
