using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.CourseOfferings.DeleteCourseOffering;

public sealed record DeleteCourseOfferingCommand(Guid OfferingId) : ICommand;
