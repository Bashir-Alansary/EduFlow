using EduFlow.Data;
using EduFlow.Entities;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Repositories.Implementations
{
    public class SectionRepository : ISectionRepository
    {
        private readonly AppDbContext _context;

        public SectionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Section>> GetAllAsync()
        {
            return await _context.Sections.ToListAsync();
        }

        public async Task<Section?> GetByIdAsync(int id)
        {
            return await _context.Sections
                .Include(s => s.Lessons)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Section>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Sections
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.Order)
                .ToListAsync();
        }

        public async Task AddAsync(Section entity)
        {
            await _context.Sections.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Update(Section entity)
        {
            _context.Sections.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(Section entity)
        {
            _context.Sections.Remove(entity);
            _context.SaveChanges();
        }
    }
}
