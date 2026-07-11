using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.CourseOfferings.CancelCourseOffering;

public sealed record CancelCourseOfferingCommand(Guid OfferingId) : ICommand;
