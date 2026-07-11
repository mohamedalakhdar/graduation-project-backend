using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;
namespace CollegeControlSystem.Application.Courses.RemovePrerequisite
{
    internal sealed class RemovePrerequisiteCommandHandler : ICommandHandler<RemovePrerequisiteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemovePrerequisiteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemovePrerequisiteCommand request, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdWithPrerequisitesAsync(request.CourseId, cancellationToken);
            if (course is null) return Result.Failure(CourseErrors.CourseNotFound);

            var result = course.RemovePrerequisite(request.PrerequisiteCourseId);

            if (result.IsFailure) return result;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
