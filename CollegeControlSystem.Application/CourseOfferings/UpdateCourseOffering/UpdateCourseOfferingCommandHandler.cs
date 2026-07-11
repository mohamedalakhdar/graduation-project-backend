using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;

namespace CollegeControlSystem.Application.CourseOfferings.UpdateCourseOffering;

internal sealed class UpdateCourseOfferingCommandHandler : ICommandHandler<UpdateCourseOfferingCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseOfferingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCourseOfferingCommand request, CancellationToken cancellationToken)
    {
        var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(request.OfferingId, cancellationToken);

        if (offering is null)
            return Result.Failure(CourseOfferingErrors.OfferingNotFound);

        var capacityResult = offering.UpdateCapacity(request.NewCapacity);
        if (capacityResult.IsFailure)
            return capacityResult;

        var instructorResult = offering.ChangeInstructor(request.NewInstructorId);
        if (instructorResult.IsFailure)
            return instructorResult;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
