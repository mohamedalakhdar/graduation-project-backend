using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.CourseOfferings.ChangeInstructor
{
    public sealed record ChangeInstructorCommand(Guid OfferingId, Guid NewInstructorId) : ICommand;
}
