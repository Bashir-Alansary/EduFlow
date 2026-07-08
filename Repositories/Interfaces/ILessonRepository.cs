using EduFlow.Entities;

namespace EduFlow.Repositories.Interfaces
{
    public interface ILessonRepository : IRepository<Lesson>
    {
        Task<List<Lesson>> GetBySectionIdAsync(int sectionId);
    }
}
