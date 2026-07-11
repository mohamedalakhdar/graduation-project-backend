namespace CollegeControlSystem.Presentation.Controllers.CourseOfferings
{
    public record CreateCourseOfferingRequest(
        Guid CourseId,
        Guid InstructorId,
        string Term,   // "Fall", "Spring", "Summer"
        int Year,
        int Capacity);
}
