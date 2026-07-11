using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;

namespace CollegeControlSystem.Application.CourseOfferings.CancelCourseOffering;

internal sealed class CancelCourseOfferingCommandHandler : ICommandHandler<CancelCourseOfferingCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelCourseOfferingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelCourseOfferingCommand request, CancellationToken cancellationToken)
    {
        var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(request.OfferingId, cancellationToken);

        if (offering is null)
            return Result.Failure(CourseOfferingErrors.OfferingNotFound);

        var result = offering.Cancel();

        if (result.IsFailure)
            return Result.Failure(result.Error);

        _unitOfWork.CourseOfferingRepository.Update(offering);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
