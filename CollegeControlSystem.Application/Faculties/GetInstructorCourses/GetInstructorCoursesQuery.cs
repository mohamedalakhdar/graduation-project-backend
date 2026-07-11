using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Faculties.GetInstructorCourses
{
    public sealed record GetInstructorCoursesQuery(Guid InstructorId) : IQuery<GetInstructorCoursesQueryResponse>;
}
