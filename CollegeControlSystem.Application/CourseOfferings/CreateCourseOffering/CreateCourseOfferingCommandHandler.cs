using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Application.CourseOfferings.CreateCourseOffering
{
    internal sealed class CreateCourseOfferingCommandHandler : ICommandHandler<CreateCourseOfferingCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseOfferingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCourseOfferingCommand request, CancellationToken cancellationToken)
        {
            // 1. Create Semester Value Object
            var semesterResult =  Semester.Create(request.Term, request.Year);
            if(semesterResult.IsFailure)
            {
                return Result<Guid>.Failure(semesterResult.Error);
            }   
            var semester = semesterResult.Value;

            // 2. Check for Duplicates (Optional but recommended)
            // Ideally, check if this Course + Semester + Instructor combination already exists to prevent double booking.
           var courseOffering= await _unitOfWork.CourseOfferingRepository.ExistsAsync(request.CourseId,request.InstructorId,semester, cancellationToken);
            if(courseOffering) {
                return Result<Guid>.Failure(CourseOfferingErrors.DuplicateOffering);
            }

            // 3. Create Domain Entity
            var result = CourseOffering.Create(
                request.CourseId,
                request.InstructorId,
                semester,
                request.Capacity
            );

            if (result.IsFailure)
            {
                return Result<Guid>.Failure(result.Error);
            }

            // 4. Persist
            _unitOfWork.CourseOfferingRepository.Add(result.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(result.Value.Id);
        }
    }
}
