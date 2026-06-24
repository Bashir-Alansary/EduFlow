using EduFlow.Entities;

namespace EduFlow.Repositories.Interfaces
{
    public interface IWishlistRepository
    {
        Task<bool> IsInWishlistAsync(string studentId, int courseId);

        Task AddAsync(Wishlist wishlist);

        Task RemoveAsync(string studentId, int courseId);

        Task<IEnumerable<Wishlist>> GetByStudentIdAsync(string studentId);
    }
}
