using EduFlow.Data;
using EduFlow.Entities;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Repositories.Implementations
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
        {
            _context  = context;
        }

        public async Task<bool> IsInWishlistAsync(string studentId, int courseId)
        {
            return await _context.Wishlists
                .AnyAsync(w =>
                    w.StudentId == studentId &&
                    w.CourseId == courseId);
        }

        public async Task AddAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(string studentId, int courseId)
        {
            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w =>
                    w.StudentId == studentId &&
                    w.CourseId == courseId);

            if (wishlistItem == null)
                return;

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Wishlist>> GetByStudentIdAsync(string studentId)
        {
            return await _context.Wishlists
                .Where(w => w.StudentId == studentId)
                .Include(w => w.Course)
                .ToListAsync();
        }

        public async Task RemoveAllAsync(string studentId)
        {
            var wishlistItems = await _context.Wishlists
                .Where(w => w.StudentId == studentId)
                .ToListAsync();

            _context.Wishlists.RemoveRange(wishlistItems);

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync(string studentId)
        {
            return await _context.Wishlists
                .CountAsync(w => w.StudentId == studentId);
        }
    }
}
