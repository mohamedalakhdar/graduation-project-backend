using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;

namespace CollegeControlSystem.Application.Courses.GetCourseDetails
{
    internal sealed class GetCourseDetailsQueryHandler : IQueryHandler<GetCourseDetailsQuery, CourseDetailsQueryResponse>
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseDetailsQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<Result<CourseDetailsQueryResponse>> Handle(GetCourseDetailsQuery request, CancellationToken cancellationToken)
        {
            // Ensure Repository.GetByIdAsync Includes Prerequisites -> PrerequisiteCourse
            var course = await _courseRepository.GetByIdWithPrerequisitesAsync(request.CourseId, cancellationToken);

            if (course is null) return Result<CourseDetailsQueryResponse>.Failure(CourseErrors.CourseNotFound);

            var prereqs = course.Prerequisites.Select(p => new PrerequisiteResponse(
                p.PrerequisiteCourseId,
                p.PrerequisiteCourse.Code.Value,
                p.PrerequisiteCourse.Title
            )).ToList();

            // If it does, you can access p.PrerequisiteCourse.Code.Value

            var response = new CourseDetailsQueryResponse(
                course.Id,
                course.Code.Value,
                course.Title,
                course.Description ?? "",
                course.Credits,
                course.LectureHours,
                course.LabHours,
                course.DepartmentId.ToString(),
                prereqs
            );

            return Result<CourseDetailsQueryResponse>.Success(response);
        }
    }
}
