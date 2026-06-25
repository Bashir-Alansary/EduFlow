namespace EduFlow.ViewModels.Sections
{
    public class SectionVM
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public int Order { get; set; }

        public int LessonsCount { get; set; }
    }
}
