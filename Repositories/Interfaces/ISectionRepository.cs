using EduFlow.Entities;

namespace EduFlow.Repositories.Interfaces
{
    public interface ISectionRepository : IRepository<Section>
    {
        Task<IEnumerable<Section>> GetByCourseIdAsync(int courseId);
    }
}
