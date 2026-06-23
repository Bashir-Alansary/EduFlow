using EduFlow.Entities;

namespace EduFlow.Repositories.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<List<Course>> GetByInstructorIdAsync(string instructorId);
        Task<Course?> GetDetailsAsync(int id);
        Task<IEnumerable<Course>> FilterAsync(string? searchTerm, int? categoryId);
    }
}
