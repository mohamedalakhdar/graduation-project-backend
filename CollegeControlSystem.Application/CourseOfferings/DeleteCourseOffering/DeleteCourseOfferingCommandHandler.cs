using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;

namespace CollegeControlSystem.Application.CourseOfferings.DeleteCourseOffering;

internal sealed class DeleteCourseOfferingCommandHandler : ICommandHandler<DeleteCourseOfferingCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourseOfferingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCourseOfferingCommand request, CancellationToken cancellationToken)
    {
        var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(request.OfferingId, cancellationToken);

        if (offering is null)
            return Result.Failure(CourseOfferingErrors.OfferingNotFound);

        var registrations = await _unitOfWork.RegistrationRepository.GetByOfferingIdAsync(request.OfferingId, cancellationToken);
        if (registrations.Count > 0)
            return Result.Failure(CourseOfferingErrors.HasRegistrations);

        _unitOfWork.CourseOfferingRepository.Delete(offering);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
