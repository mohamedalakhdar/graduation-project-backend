using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;
namespace CollegeControlSystem.Application.Courses.CreateCourse
{
    internal sealed class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            // 1. Check Uniqueness
            if (!await _unitOfWork.CourseRepository.IsCodeUniqueAsync(request.Code, cancellationToken))
            {
                return Result<Guid>.Failure(CourseErrors.DuplicateCode);
            }

            // 2. Create Domain Entity
            var result = Course.Create(
                request.DepartmentId,
                request.Code,
                request.Title,
                request.Description,
                request.Credits,
                request.LectureHours,
                request.LabHours
            );

            if (result.IsFailure)
            {
                return Result<Guid>.Failure(result.Error);
            }

            var course = result.Value;

            // 3. Persist
            _unitOfWork.CourseRepository.Add(course);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(course.Id);
        }
    }
}
