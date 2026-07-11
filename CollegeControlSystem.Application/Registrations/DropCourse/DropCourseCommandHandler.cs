using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;
namespace CollegeControlSystem.Application.Registrations.DropCourse
{
    internal sealed class DropCourseCommandHandler : ICommandHandler<DropCourseCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DropCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DropCourseCommand request, CancellationToken cancellationToken)
        {
            // 1. Get Registration (via UoW)
            var registration = await _unitOfWork.RegistrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);

            if (registration is null)
            {
                return Result.Failure(RegistrationErrors.NotFound);
            }

            // 2. Security Check: Ownership
            if (registration.StudentId != request.StudentId)
            {
                return Result.Failure(RegistrationErrors.Unauthorized);
            }

            // 3. Domain Logic: Change Status
            // The Entity handles validation (e.g., cannot drop if already 'Completed' or 'Withdrawn')
            var dropResult = registration.Drop();

            if (dropResult.IsFailure)
            {
                return dropResult;
            }

            // 4. Domain Logic: Release Seat in Offering
            // We must load the related Offering to update its "CurrentEnrolled" count
            var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(registration.CourseOfferingId, cancellationToken);

            if (offering is null)
            {
                // This is a data consistency error, but we shouldn't fail the drop if the offering is missing.
                // Log this if you have a logger.
            }
            else
            {
                offering.ReleaseSeat();
                // EF Core tracking will detect this change automatically
            }

            // 5. Commit Transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
