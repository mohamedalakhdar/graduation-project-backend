using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.CourseOfferings.GetRoster
{
    public sealed record GetCourseRosterQuery(Guid OfferingId) : IQuery<List<StudentRosterResponse>>;
}
