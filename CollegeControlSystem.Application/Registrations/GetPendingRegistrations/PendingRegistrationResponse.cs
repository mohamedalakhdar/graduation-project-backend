
namespace CollegeControlSystem.Application.Registrations.GetPendingRegistrations
{
    public sealed record PendingRegistrationResponse(
        Guid RegistrationId,
        Guid StudentId,
        string StudentName,
        string AcademicNumber,
        string CourseCode,
        string CourseTitle,
        string Term,
        int Year,
        DateTime RequestDate
    );
}
