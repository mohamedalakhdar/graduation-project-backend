using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Courses.GetCourseDetails;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;

namespace CollegeControlSystem.Application.Courses.GetPrerequisites;

public sealed record GetPrerequisitesQuery(Guid CourseId) : IQuery<List<PrerequisiteResponse>>;
