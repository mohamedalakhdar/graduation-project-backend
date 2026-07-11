using CollegeControlSystem.Application.Abstractions.Messaging;

namespace CollegeControlSystem.Application.Courses.GetCourseDetails
{
    public sealed record GetCourseDetailsQuery(Guid CourseId) : IQuery<CourseDetailsQueryResponse>;

}
