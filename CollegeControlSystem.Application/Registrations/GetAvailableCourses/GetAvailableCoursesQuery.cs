using CollegeControlSystem.Application.Abstractions.Messaging;
namespace CollegeControlSystem.Application.Registrations.GetAvailableCourses
{
    public sealed record GetAvailableCoursesQuery(
            Guid StudentId,
            string Term,
            int Year) : IQuery<List<AvailableCourseResponse>>;
}
