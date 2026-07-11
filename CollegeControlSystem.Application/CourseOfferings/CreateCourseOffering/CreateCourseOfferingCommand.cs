using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.CourseOfferings.CreateCourseOffering
{
    public sealed record CreateCourseOfferingCommand(
        Guid CourseId,
        Guid InstructorId,
        string Term,   // Enum: Fall, Spring, Summer
        int Year,    // e.g., 2025
        int Capacity
    ) : ICommand<Guid>;
}
