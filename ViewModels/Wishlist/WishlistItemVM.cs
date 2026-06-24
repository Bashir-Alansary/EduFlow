namespace EduFlow.ViewModels.Wishlist
{
    public class WishlistItemVM
    {
        public int CourseId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}
