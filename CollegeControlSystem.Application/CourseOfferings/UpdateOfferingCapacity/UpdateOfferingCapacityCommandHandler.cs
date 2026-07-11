using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;

namespace CollegeControlSystem.Application.CourseOfferings.UpdateOfferingCapacity
{
    internal sealed class UpdateOfferingCapacityCommandHandler : ICommandHandler<UpdateOfferingCapacityCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOfferingCapacityCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateOfferingCapacityCommand request, CancellationToken cancellationToken)
        {
            var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(request.OfferingId, cancellationToken);

            if (offering is null)
            {
                return Result.Failure(CourseOfferingErrors.OfferingNotFound);
            }

            // Domain Logic checks if NewCapacity < CurrentEnrolled
            var result = offering.UpdateCapacity(request.NewCapacity);

            if (result.IsFailure) return result;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
