namespace CollegeControlSystem.Presentation.Controllers.CourseOfferings;

public record UpdateCourseOfferingRequest(
    int NewCapacity,
    Guid NewInstructorId
);
