using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;

namespace CollegeControlSystem.Application.Courses.DeleteCourse;

internal sealed class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
            return Result.Failure(CourseErrors.CourseNotFound);

        var offerings = await _unitOfWork.CourseOfferingRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        if (offerings.Count > 0)
            return Result.Failure(CourseErrors.HasOfferings);

        foreach (var offering in offerings)
        {
            var registrations = await _unitOfWork.RegistrationRepository.GetByOfferingIdAsync(offering.Id, cancellationToken);
            if (registrations.Count > 0)
                return Result.Failure(CourseErrors.HasRegistrations);
        }

        _unitOfWork.CourseRepository.Delete(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
