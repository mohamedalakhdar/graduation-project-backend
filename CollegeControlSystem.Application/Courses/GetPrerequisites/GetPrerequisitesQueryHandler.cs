using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Courses.GetCourseDetails;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;

namespace CollegeControlSystem.Application.Courses.GetPrerequisites;

internal sealed class GetPrerequisitesQueryHandler : IQueryHandler<GetPrerequisitesQuery, List<PrerequisiteResponse>>
{
    private readonly ICourseRepository _courseRepository;

    public GetPrerequisitesQueryHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<Result<List<PrerequisiteResponse>>> Handle(GetPrerequisitesQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdWithPrerequisitesAsync(request.CourseId, cancellationToken);

        if (course is null)
            return Result<List<PrerequisiteResponse>>.Failure(CourseErrors.CourseNotFound);

        var prereqs = course.Prerequisites
            .Select(p => new PrerequisiteResponse(
                p.PrerequisiteCourseId,
                p.PrerequisiteCourse.Code.Value,
                p.PrerequisiteCourse.Title))
            .ToList();

        return Result<List<PrerequisiteResponse>>.Success(prereqs);
    }
}
