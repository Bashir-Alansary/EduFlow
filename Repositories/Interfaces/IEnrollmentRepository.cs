using EduFlow.Entities;

namespace EduFlow.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<bool> IsEnrolledAsync(string studentId, int courseId);
        Task EnrollAsync(Enrollment enrollment);
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId);
    }
}
