using EduFlow.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EduFlow.Services.Implementations
{
    public class CertificateService : ICertificateService
    {
        public byte[] GenerateCertificate(
            string studentName,
            string courseTitle,
            string instructorName,
            DateTime completedDate)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(35);

                    page.Content()
                        .Border(3)
                        .BorderColor(Colors.Blue.Medium)
                        .Padding(35)
                        .Column(col =>
                        {
                            col.Spacing(20);

                        col.Item()
                            .AlignCenter()
                            .Text("CERTIFICATE OF COMPLETION")
                            .FontSize(34)
                            .FontColor(Colors.Blue.Darken2)
                            .Bold();

                            col.Item()
                            .AlignCenter()
                            .Text("This certificate is proudly presented to")
                            .FontSize(16);

                        col.Item()
                            .AlignCenter()
                            .Text(studentName)
                            .FontSize(30)
                            .FontColor(Colors.Green.Darken2)
                            .Bold();

                            col.Item()
                            .AlignCenter()
                            .Text("For successfully completing")
                            .FontSize(16);

                        col.Item()
                            .AlignCenter()
                            .Text(courseTitle)
                            .FontSize(26)
                            .FontColor(Colors.Blue.Medium)
                            .Bold();

                            col.Item().PaddingTop(20);

                        col.Item()
                            .Text($"Instructor: {instructorName}")
                            .FontSize(16);

                        col.Item()
                            .Text($"Completed On: {completedDate:dd MMMM yyyy}")
                            .FontSize(16);

                        col.Item().PaddingTop(40);

                        col.Item()
                            .AlignCenter()
                            .Text("EduFlow")
                            .FontSize(24)
                            .FontColor(Colors.Blue.Darken3)
                            .Bold();
                        });
                });
            }).GeneratePdf();
        }
    }
}