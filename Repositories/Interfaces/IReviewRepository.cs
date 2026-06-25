using EduFlow.Entities;

namespace EduFlow.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> HasReviewedAsync(string studentId, int courseId);

        Task AddAsync(Review review);

        Task<IEnumerable<Review>> GetCourseReviewsAsync(int courseId);
        Task<int> GetStudentReviewsCountAsync(string studentId);
    }
}
