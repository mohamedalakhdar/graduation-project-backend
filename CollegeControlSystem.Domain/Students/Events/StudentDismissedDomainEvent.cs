using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Domain.Students.Events
{
    public record StudentDismissedDomainEvent(
    Guid StudentId,
    string AcademicNumber,
    string Reason,
    int ConsecutiveWarningsCount,
    DateTime OccurredOn
) : IDomainEvent;

}
