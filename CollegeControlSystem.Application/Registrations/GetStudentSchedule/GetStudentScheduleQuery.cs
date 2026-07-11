using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Registrations.GetStudentSchedule
{
    public sealed record GetStudentScheduleQuery(
        Guid StudentId,
        string? Term = null,
        int? Year = null
    ) : IQuery<ScheduleResponse>;
}
