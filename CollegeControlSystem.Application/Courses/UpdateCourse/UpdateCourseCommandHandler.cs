using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;

namespace CollegeControlSystem.Application.Courses.UpdateCourse;

internal sealed class UpdateCourseCommandHandler : ICommandHandler<UpdateCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure(CourseErrors.CourseNotFound);

        var result = course.Update(
            request.DepartmentId,
            request.Title,
            request.Description,
            request.Credits,
            request.LectureHours,
            request.LabHours);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        _unitOfWork.CourseRepository.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
