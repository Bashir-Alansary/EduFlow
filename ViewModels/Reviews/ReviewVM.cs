namespace EduFlow.ViewModels.Reviews
{
    public class ReviewVM
    {
        public string StudentName { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
