namespace EduFlow.Services.Interfaces
{
    public interface ICertificateService
    {
        byte[] GenerateCertificate(
            string studentName,
            string courseTitle,
            string instructorName,
            DateTime completedDate);
    }
}
