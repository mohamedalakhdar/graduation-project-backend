namespace CollegeControlSystem.Presentation.Controllers.Registratoins
{
    // Similarly, StudentId serves as a safety check here to ensure the user owns the registration
    public record DropCourseRequest(Guid StudentId);

}
