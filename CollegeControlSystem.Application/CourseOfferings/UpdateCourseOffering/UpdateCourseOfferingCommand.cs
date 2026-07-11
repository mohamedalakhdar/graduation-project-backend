using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.CourseOfferings.UpdateCourseOffering;

public sealed record UpdateCourseOfferingCommand(
    Guid OfferingId,
    int NewCapacity,
    Guid NewInstructorId
) : ICommand;
