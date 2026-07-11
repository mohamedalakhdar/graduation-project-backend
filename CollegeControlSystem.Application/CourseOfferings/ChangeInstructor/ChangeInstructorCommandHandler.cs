using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;

namespace CollegeControlSystem.Application.CourseOfferings.ChangeInstructor
{
    internal sealed class ChangeInstructorCommandHandler : ICommandHandler<ChangeInstructorCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeInstructorCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ChangeInstructorCommand request, CancellationToken cancellationToken)
        {
            var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(request.OfferingId, cancellationToken);

            if (offering is null) return Result.Failure(CourseOfferingErrors.OfferingNotFound);

            var result = offering.ChangeInstructor(request.NewInstructorId);

            if (result.IsFailure) return result;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
