
namespace CollegeControlSystem.Application.Registrations.GetStudentSchedule
{
    public sealed record ScheduleResponse(
        Guid StudentId,
        string StudentName,
        string Term,
        int Year,
        int TotalCredits,
        List<ClassScheduleItem> Classes
    );

    public sealed record ClassScheduleItem(
        Guid CourseId,
        string CourseCode,
        string CourseTitle,
        string InstructorName,
        int Credits,
        string Status // e.g., "Approved", "Pending"
    );
}
