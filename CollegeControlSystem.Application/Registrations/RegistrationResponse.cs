namespace CollegeControlSystem.Application.Registrations
{
    public sealed record RegistrationResponse(
        Guid Id,
        Guid StudentId,
        string? StudentName,
        Guid CourseOfferingId,
        string? CourseCode,
        string? CourseTitle,
        string Status,
        bool IsRetake,
        DateTime RegistrationDate,
        string? GradeLetter
    );
}
