using EduFlow.Entities;

namespace EduFlow.Repositories.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<List<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int id);

        Task AddAsync(Course course);
        void Update(Course course);
        void Delete(Course course);
    }
}
