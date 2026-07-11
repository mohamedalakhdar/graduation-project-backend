namespace CollegeControlSystem.Application.Faculties.GetInstructorCourses
{
    public class GetInstructorCoursesQueryResponse
    {
        public Guid InstructorId { get; set; }
        public required IList<InstructorCourseResponse> Courses { get; set; }
    }

    public sealed record InstructorCourseResponse(
        Guid OfferingId,
        string CourseCode,
        string CourseTitle,
        string Semester,
        int Year,
        int EnrolledCount,
        int Capacity
    );
}
